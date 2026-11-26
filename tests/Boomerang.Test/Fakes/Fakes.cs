using Boomerang.Application.Interfaces;
using Boomerang.Application.Players;
using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Editions.Shared.DefaultRules;
using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;
using Boomerang.Domain.Strategies;

namespace Boomerang.Test.Fakes;

/// <summary>Leaves the deck in its original order.</summary>
public sealed class IdentityShuffler : IShuffler
{
    public int Calls { get; private set; }
    public void Shuffle<T>(IList<T> items) => Calls++;
}

/// <summary>Reverses the deck.</summary>
public sealed class ReverseShuffler : IShuffler
{
    public void Shuffle<T>(IList<T> items)
    {
        var copy = items.Reverse().ToList();
        for (int i = 0; i < copy.Count; i++) items[i] = copy[i];
    }
}

/// <summary>
/// Tiny edition whose scores can be worked out by hand: nine cards A-I (numbers 1,2,9,4,5,6,7,8,3),
/// regions R1 = {A,C,E}, R2 = {B,D,F,G}, R3 = {H,I}, only Throw &amp; Catch and Tourist sites scoring,
/// and a region bonus of 10.
/// </summary>
public sealed class FakeEdition : IGameEdition
{
    private static readonly int[] Numbers = { 1, 2, 9, 4, 5, 6, 7, 8, 3 };

    public FakeEdition(int handSize = 3, int rounds = 2)
    {
        HandSize = handSize;
        Rounds = rounds;
        Cards = "ABCDEFGHI"
            .Select((site, i) => new Card($"Site {site}", site.ToString(), RegionOf(site), Numbers[i], Array.Empty<Symbol>()))
            .ToList();
        Regions = new[]
        {
            new Region("R1", new[] { "A", "C", "E" }),
            new Region("R2", new[] { "B", "D", "F", "G" }),
            new Region("R3", new[] { "H", "I" })
        };
        ScoringRules = new IScoringRule[] { new ThrowAndCatchRule(), new TouristSitesRule() };
    }

    private static string RegionOf(char site) => site switch
    {
        'A' or 'C' or 'E' => "R1",
        'B' or 'D' or 'F' or 'G' => "R2",
        _ => "R3"
    };

    public string Name => "Fake edition";
    public IReadOnlyList<Card> Cards { get; }
    public IReadOnlyList<Region> Regions { get; }
    public IReadOnlyList<IScoringRule> ScoringRules { get; }
    public int Rounds { get; }
    public int HandSize { get; }
    public int MinPlayers => 2;
    public int MaxPlayers => 4;
    public int RegionBonus => 10;
    public string TieBreakCategory => ScoreCategories.ThrowAndCatch;
}

public sealed class FakeEditionResolver : IEditionResolver
{
    private readonly IGameEdition _edition;
    public FakeEditionResolver(IGameEdition edition) => _edition = edition;
    public IGameEdition Resolve(string edition) => _edition;
}

/// <summary>Always picks from the hand through a chooser function (default: first card) and records everything.</summary>
public sealed class FakePlayerInteraction : IPlayerInteraction
{
    private readonly Func<PickContext, Card> _choose;

    public FakePlayerInteraction(Func<PickContext, Card>? choose = null) =>
        _choose = choose ?? (ctx => ctx.Hand[0]);

    public List<PickContext> Picks { get; } = new();
    public List<IReadOnlyList<PlayerReveal>> Reveals { get; } = new();
    public List<RoundResult> Rounds { get; } = new();
    public GameResult? Finished { get; private set; }

    public Task<Card> ChooseCardAsync(PickContext context, CancellationToken cancellationToken = default)
    {
        Picks.Add(context);
        return Task.FromResult(_choose(context));
    }

    public Task<string?> ChooseOptionalScoreAsync(string category, IReadOnlyList<ScoreOption> options, CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>(null);

    public Task OnCardsRevealedAsync(IReadOnlyList<PlayerReveal> reveals, CancellationToken cancellationToken = default)
    {
        Reveals.Add(reveals);
        return Task.CompletedTask;
    }

    public Task OnRoundScoredAsync(RoundResult result, CancellationToken cancellationToken = default)
    {
        Rounds.Add(result);
        return Task.CompletedTask;
    }

    public Task OnGameFinishedAsync(GameResult result, CancellationToken cancellationToken = default)
    {
        Finished = result;
        return Task.CompletedTask;
    }
}