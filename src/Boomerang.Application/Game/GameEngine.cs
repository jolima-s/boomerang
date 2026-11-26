using Boomerang.Application.Models;
using Boomerang.Domain;
using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;
using Boomerang.Domain.Strategies;
using System.Data;

namespace Boomerang.Application.Game;

public sealed class GameEngine
{
    private readonly IGameEdition _edition;
    private readonly GameState _gameState;
    private readonly IReadOnlyList<Player> _players;
    private readonly IShuffler _shuffler;
    private readonly IPassingStrategy _passing;
    private readonly RegionBonusTracker _regions;

    public GameEngine(IGameEdition edition,
        GameState gameState,
        IShuffler shuffler,
        IPassingStrategy passing)
    {
        _gameState = gameState;
        _edition = edition;
        _players = _gameState.Players;
        _shuffler = shuffler;
        _passing = passing;
        _regions = new RegionBonusTracker(_edition.Regions);

        if (_players.Count < _edition.MinPlayers || _players.Count > _edition.MaxPlayers)
            throw new GameSetupException(
                $"{_edition.Name} is for {_edition.MinPlayers}-{_edition.MaxPlayers} players, but {_players.Count} were given.");
        if (_edition.HandSize < 2)
            throw new GameSetupException("A hand needs at least two cards (a Throw and a Catch).");
        if (_edition.Cards.Count < _players.Count * _edition.HandSize)
            throw new GameSetupException("The deck is too small to deal a hand to every player.");
        for (int i = 0; i < _players.Count; i++)
            if (_players[i].Index != i)
                throw new GameSetupException("Player indexes must match their seating order."); 
    }

    public async Task<GameResult> PlayAsync(CancellationToken cancellationToken = default)
    {
        for (int round = 0; round < _edition.Rounds; round++)
            await PlayRoundAsync(round, cancellationToken);

        GameResult result = BuildResult();
        await Task.WhenAll(_players.Select(p => p.Notifications.OnGameFinishedAsync(result, cancellationToken)));
        return result;
    }

    private async Task PlayRoundAsync(int round, CancellationToken cancellationToken)
    {
        List<List<Card>> hands = Deal();
        List<List<Card>> drafts = _players.Select(_ => new List<Card>()).ToList();
        PassDirection direction = _passing.DirectionFor(round);

        await PickAsync(round, PickKind.Throw, hands, drafts, cancellationToken); 

        while (true)
        {
            hands = PassHands(hands, direction);
            await PickAsync(round, PickKind.Draft, hands, drafts, cancellationToken);
            await RevealAsync(drafts, cancellationToken);
            if (hands[0].Count == 1) break;
        }

        hands = PassHands(hands, direction.Opposite());
        for (int i = 0; i < _players.Count; i++)
            drafts[i].Add(hands[i].Single());

        await ScoreRoundAsync(round, drafts, cancellationToken); 
    }

    private List<List<Card>> Deal()
    {
        List<Card> deck = _edition.Cards.ToList();
        _shuffler.Shuffle(deck);

        var hands = new List<List<Card>>();
        for (int i = 0; i < _players.Count; i++)
        {
            hands.Add(deck.Take(_edition.HandSize).ToList());
            deck.RemoveRange(0, _edition.HandSize);
        }
        return hands;
    }

    private static List<List<Card>> PassHands(List<List<Card>> hands, PassDirection direction)
    {
        var passed = hands.Select(_ => new List<Card>()).ToList();
        for (int i = 0; i < hands.Count; i++)
            passed[direction.ReceiverOf(i, hands.Count)] = hands[i];
        return passed;
    }

    /// <summary>All players choose at the same time; the choices are applied once everybody has answered.</summary>
    private async Task PickAsync(int round, PickKind kind, List<List<Card>> hands, List<List<Card>> drafts, CancellationToken cancellationToken)
    {
        var tasks = Enumerable.Range(0, _players.Count).Select(i =>
        {
            var context = new PickContext(
                round + 1,
                _edition.Rounds,
                kind,
                hands[i].ToList(),
                kind == PickKind.Throw ? null : drafts[i][0],
                drafts[i].Skip(1).ToList(),
                _players[i].Sheet.VisitedSites.ToList());
            return _players[i].Decisions.ChooseCardAsync(context, cancellationToken);
        }).ToList();

        Card[] chosen = await Task.WhenAll(tasks);

        for (int i = 0; i < _players.Count; i++)
        {
            if (!hands[i].Remove(chosen[i]))
                throw new InvalidMoveException($"{_players[i].Name} chose {chosen[i]}, which is not in their hand.");
            drafts[i].Add(chosen[i]);
        }
    }

    /// <summary>Show everybody's face-up cards. The Throw card (index 0) stays hidden until the round ends.</summary>
    private Task RevealAsync(List<List<Card>> drafts, CancellationToken cancellationToken)
    {
        IReadOnlyList<PlayerReveal> reveals = _players
            .Select(p => new PlayerReveal(p.Index, p.Name, drafts[p.Index].Skip(1).ToList()))
            .ToList();
        return Task.WhenAll(_players.Select(p => p.Notifications.OnCardsRevealedAsync(reveals, cancellationToken)));
    }

    private async Task ScoreRoundAsync(int round, List<List<Card>> drafts, CancellationToken cancellationToken)
    {
        // Region bonuses consider the sites of this round as well as all earlier rounds.
        var visitedAfterRound = _players
            .Select(p => new HashSet<string>(p.Sheet.VisitedSites.Concat(drafts[p.Index].Select(c => c.Site))))
            .ToList();
        IReadOnlyList<IReadOnlyList<Region>> bonuses = _regions.Award(visitedAfterRound);

        foreach (Player player in _players)
        {
            List<Card> draft = drafts[player.Index];
            var context = new RoundContext(draft, player.Sheet);
            var entries = new List<ScoreEntry>();

            foreach (IScoringRule rule in _edition.ScoringRules)
                entries.Add(await ScoreRuleAsync(player, rule, context, cancellationToken));

            IReadOnlyList<Region> won = bonuses[player.Index];
            entries.Add(new ScoreEntry(
                ScoreCategories.RegionBonus,
                won.Count * _edition.RegionBonus,
                won.Count == 0 ? null : string.Join(", ", won.Select(r => r.Name))));

            player.Sheet.MarkVisited(draft.Select(c => c.Site));
            var score = new RoundScore(entries);
            player.Sheet.AddRound(score);

            await player.Notifications.OnRoundScoredAsync(new RoundResult(
                round + 1, _edition.Rounds, draft, score, player.Sheet.Total, _regions.ClaimedRegions.ToList()), cancellationToken);
        }
    }

    private static async Task<ScoreEntry> ScoreRuleAsync(Player player, IScoringRule rule, RoundContext context, CancellationToken cancellationToken)
    {
        RuleOutcome outcome = rule.Evaluate(context);
        if (outcome.Options.Count == 0)
            return new ScoreEntry(rule.Category, outcome.Points);

        string? key = await player.Decisions.ChooseOptionalScoreAsync(rule.Category, outcome.Options, cancellationToken);
        if (key is null)
            return new ScoreEntry(rule.Category, 0);

        ScoreOption option = outcome.Options.FirstOrDefault(o => o.Key == key)
            ?? throw new InvalidMoveException($"{player.Name} chose '{key}', which was not offered.");
        player.Sheet.Claim(option.Key);
        return new ScoreEntry(rule.Category, option.Points, option.Label);
    }

    private GameResult BuildResult()
    {
        List<string> categories = _edition.ScoringRules.Select(r => r.Category)
            .Append(ScoreCategories.RegionBonus).ToList();

        List<PlayerResult> results = _players.Select(p => new PlayerResult(
            p.Index,
            p.Name,
            p.Sheet.Total,
            p.Sheet.Rounds.ToList(),
            categories.ToDictionary(c => c, c => p.Sheet.CategoryTotal(c)))).ToList();

        return new GameResult(categories, results, WinnerSelector.Select(results, _edition.TieBreakCategory));
    }
}
