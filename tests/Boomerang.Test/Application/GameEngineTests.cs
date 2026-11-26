using Boomerang.Application.Game;
using Boomerang.Application.Models;
using Boomerang.Domain;
using Boomerang.Domain.Editions.Australia;
using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Strategies;
using Boomerang.Test.Fakes;

namespace Boomerang.Test.Application;

public class GameEngineTests
{
    private static (GameEngine engine, FakePlayerInteraction[] players, IdentityShuffler shuffler) Create(
        IGameEdition? edition = null,
        Func<PickContext, Card>? chooser0 = null,
        int playerCount = 2,
        IPassingStrategy? passing = null)
    {
        edition ??= new AustraliaEdition();
        var state = new GameState();
        var fakes = Enumerable.Range(0, playerCount)
            .Select(i => new FakePlayerInteraction(i == 0 ? chooser0 : null)).ToArray();
        for (int i = 0; i < playerCount; i++)
            state.AddPlayer(new Player(i, "P" + i, fakes[i]));
        var shuffler = new IdentityShuffler();
        var engine = new GameEngine(edition, state, shuffler, passing ?? new PassNextStrategy());
        return (engine, fakes, shuffler);
    }

    private static string[] Sites(IEnumerable<Card> cards) => cards.Select(c => c.Site).ToArray();

    // ---------------------------------------------------------------------------------------------
    // Exact scores. With FakeEdition, the identity shuffler, two players and "always pick the first
    // card", the deal and every pick are known:
    //   P0 hand A B C, P1 hand D E F. Throw: P0 A, P1 D. Hands are passed, P0 sees E F and P1 sees B C.
    //   Draft: P0 E, P1 B. The last cards (P0 holds F, P1 holds C) are passed back, so P0 catches C, P1 catches F.
    //   P0 draft A,E,C   P1 draft D,B,F
    // ---------------------------------------------------------------------------------------------

    [Fact]
    public async Task PlayAsync_FakeEdition_ScoresEveryCategoryExactly()
    {
        var (engine, players, shuffler) = Create(new FakeEdition());

        GameResult result = await engine.PlayAsync();

        Assert.Equal(2, shuffler.Calls);
        var p0 = players[0].Rounds;
        var p1 = players[1].Rounds;
        Assert.Equal(2, p0.Count);
        Assert.Equal(2, p1.Count);

        // Round 1: P0 Throw A(1) / Catch C(9) = 8, three new sites, and R1 = {A,C,E} completed (+10).
        Assert.Equal(new[] { "A", "E", "C" }, Sites(p0[0].Draft));
        Assert.Equal(8, p0[0].Score.PointsFor(ScoreCategories.ThrowAndCatch));
        Assert.Equal(3, p0[0].Score.PointsFor(ScoreCategories.TouristSites));
        Assert.Equal(10, p0[0].Score.PointsFor(ScoreCategories.RegionBonus));
        Assert.Equal(21, p0[0].RunningTotal);
        Assert.Equal(new[] { "R1" }, p0[0].ClaimedRegions);

        // Round 1: P1 Throw D(4) / Catch F(6) = 2, three new sites, no region completed (R2 also needs G).
        Assert.Equal(new[] { "D", "B", "F" }, Sites(p1[0].Draft));
        Assert.Equal(2, p1[0].Score.PointsFor(ScoreCategories.ThrowAndCatch));
        Assert.Equal(3, p1[0].Score.PointsFor(ScoreCategories.TouristSites));
        Assert.Equal(0, p1[0].Score.PointsFor(ScoreCategories.RegionBonus));
        Assert.Equal(5, p1[0].RunningTotal);

        // Round 2 is the same deal: no site scores again and R1 is not awarded a second time.
        Assert.Equal(8, p0[1].Score.PointsFor(ScoreCategories.ThrowAndCatch));
        Assert.Equal(0, p0[1].Score.PointsFor(ScoreCategories.TouristSites));
        Assert.Equal(0, p0[1].Score.PointsFor(ScoreCategories.RegionBonus));
        Assert.Equal(29, p0[1].RunningTotal);
        Assert.Equal(2, p1[1].Score.PointsFor(ScoreCategories.ThrowAndCatch));
        Assert.Equal(0, p1[1].Score.PointsFor(ScoreCategories.TouristSites));
        Assert.Equal(7, p1[1].RunningTotal);

        // Final result.
        Assert.Equal(new[] { ScoreCategories.ThrowAndCatch, ScoreCategories.TouristSites, ScoreCategories.RegionBonus }, result.Categories);
        Assert.Equal(29, result.Players[0].Total);
        Assert.Equal(7, result.Players[1].Total);
        Assert.Equal(16, result.Players[0].CategoryTotals[ScoreCategories.ThrowAndCatch]);
        Assert.Equal(3, result.Players[0].CategoryTotals[ScoreCategories.TouristSites]);
        Assert.Equal(10, result.Players[0].CategoryTotals[ScoreCategories.RegionBonus]);
        Assert.Equal(0, Assert.Single(result.Winners).PlayerIndex);
    }

    [Fact]
    public async Task PlayAsync_PlaysExactlyTheEditionsNumberOfRounds()
    {
        var edition = new FakeEdition(rounds: 3);
        var (engine, players, shuffler) = Create(edition);

        await engine.PlayAsync();

        Assert.Equal(3, shuffler.Calls);
        Assert.All(players, p =>
        {
            Assert.Equal(3, p.Rounds.Count);
            Assert.Equal(new[] { 1, 2, 3 }, p.Rounds.Select(r => r.Round));
        });
    }

    // ---------------------------------------------------------------------------------------------
    // Australian rules at engine level
    // ---------------------------------------------------------------------------------------------

    [Fact]
    public async Task PlayAsync_Australia_PlaysFourRoundsOfSixPicksAndFiveReveals()
    {
        var edition = new AustraliaEdition();
        var (engine, players, shuffler) = Create(edition);

        GameResult result = await engine.PlayAsync();

        Assert.Equal(4, shuffler.Calls);
        Assert.Same(result, players[0].Finished);
        foreach (var p in players)
        {
            Assert.Equal(4, p.Rounds.Count);
            Assert.Equal(4 * 5, p.Reveals.Count);
            Assert.Equal(4 * 6, p.Picks.Count);
            Assert.All(p.Rounds, r => Assert.Equal(7, r.Draft.Count));
        }

        // Per round: one Throw pick from 7 cards, then Draft picks from 6, 5, 4, 3 and 2 cards.
        for (int round = 1; round <= 4; round++)
        {
            var inRound = players[0].Picks.Where(p => p.Round == round).ToList();
            Assert.Equal(new[] { 7, 6, 5, 4, 3, 2 }, inRound.Select(p => p.Hand.Count));
            Assert.Equal(PickKind.Throw, inRound[0].Kind);
            Assert.Null(inRound[0].ThrowCard);
            Assert.All(inRound.Skip(1), p => Assert.Equal(PickKind.Draft, p.Kind));
            Assert.All(inRound, p => Assert.Equal(4, p.TotalRounds));
        }
    }

    [Fact]
    public async Task PlayAsync_Australia_DraftPickShowsOwnThrowCardAndGrowingFaceUpCards()
    {
        var (engine, players, _) = Create();

        await engine.PlayAsync();

        var round1 = players[0].Picks.Where(p => p.Round == 1).ToList();
        Card myThrow = round1[0].Hand[0]; // the fake picks the first card of the hand
        for (int j = 1; j < round1.Count; j++)
        {
            Assert.Equal(myThrow, round1[j].ThrowCard);
            Assert.Equal(j - 1, round1[j].FaceUp.Count);
        }
    }

    [Fact]
    public async Task PlayAsync_Australia_ThrowCardIsNeverRevealedBeforeTheRoundEnds()
    {
        var (engine, players, _) = Create(playerCount: 3);

        await engine.PlayAsync();

        for (int round = 0; round < 4; round++)
        {
            // Throw card of every player this round (the fake always picks Hand[0]).
            List<Card> throws = players
                .Select(p => p.Picks.Where(x => x.Kind == PickKind.Throw).ElementAt(round).Hand[0])
                .ToList();

            // Every player receives the same five reveals per round.
            foreach (var reveals in players[0].Reveals.Skip(round * 5).Take(5))
                foreach (PlayerReveal revealed in reveals)
                    foreach (Card hidden in throws)
                        Assert.DoesNotContain(hidden, revealed.FaceUpCards);

            // After scoring, the Throw card is the first card of the draft the player is shown.
            for (int i = 0; i < players.Length; i++)
                Assert.Equal(throws[i], players[i].Rounds[round].Draft[0]);
        }
    }

    [Fact]
    public async Task PlayAsync_Australia_DealsEveryCardOnceToFourPlayers()
    {
        var (engine, players, _) = Create(playerCount: 4);

        await engine.PlayAsync();

        var firstHands = players.Select(p => p.Picks[0].Hand).ToList();
        Assert.All(firstHands, h => Assert.Equal(7, h.Count));
        Assert.Equal(28, firstHands.SelectMany(h => h).Select(c => c.Site).Distinct().Count());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task PlayAsync_Australia_CompletesForEveryAllowedPlayerCount(int count)
    {
        var (engine, players, _) = Create(playerCount: count);

        GameResult result = await engine.PlayAsync();

        Assert.Equal(count, result.Players.Count);
        Assert.All(players, p => Assert.Equal(4, p.Rounds.Count));
        Assert.NotEmpty(result.Winners);
    }

    // ---------------------------------------------------------------------------------------------
    // Passing direction (requirement 13), checked through the engine. Three players, FakeEdition:
    //   hands P0 A B C, P1 D E F, P2 G H I; after the Throw picks P0 holds B C, P1 E F, P2 H I.
    //   Pass Next: P0 receives H I from P2 and picks H.   Pass Previous: P0 receives E F from P1 and picks E.
    //   P0 always catches C.
    // ---------------------------------------------------------------------------------------------

    [Fact]
    public async Task PlayAsync_PassNextStrategy_PassesTheSameWayEveryRound()
    {
        var (engine, players, _) = Create(new FakeEdition(), playerCount: 3, passing: new PassNextStrategy());

        await engine.PlayAsync();

        Assert.Equal(new[] { "A", "H", "C" }, Sites(players[0].Rounds[0].Draft));
        Assert.Equal(new[] { "A", "H", "C" }, Sites(players[0].Rounds[1].Draft));
    }

    [Fact]
    public async Task PlayAsync_AlternatingStrategy_ReversesDirectionEverySecondRound()
    {
        var (engine, players, _) = Create(new FakeEdition(), playerCount: 3, passing: new AlternatingPassStrategy());

        await engine.PlayAsync();

        Assert.Equal(new[] { "A", "H", "C" }, Sites(players[0].Rounds[0].Draft));
        Assert.Equal(new[] { "A", "E", "C" }, Sites(players[0].Rounds[1].Draft));
    }

    // ---------------------------------------------------------------------------------------------
    // Other engine behaviour
    // ---------------------------------------------------------------------------------------------

    [Fact]
    public async Task PlayAsync_ReverseShufflerChangesDeal()
    {
        var edition = new AustraliaEdition();
        var state = new GameState();
        var a = new FakePlayerInteraction();
        var b = new FakePlayerInteraction();
        state.AddPlayer(new Player(0, "P0", a));
        state.AddPlayer(new Player(1, "P1", b));
        var engine = new GameEngine(edition, state, new ReverseShuffler(), new PassNextStrategy());

        await engine.PlayAsync();

        var reversed = edition.Cards.Reverse().ToList();
        Assert.Equal(reversed.Take(edition.HandSize).Select(c => c.Site), a.Picks[0].Hand.Select(c => c.Site));
        Assert.Equal(reversed.Skip(edition.HandSize).Take(edition.HandSize).Select(c => c.Site), b.Picks[0].Hand.Select(c => c.Site));
    }

    [Fact]
    public async Task PlayAsync_ChoosingCardNotInHand_Throws()
    {
        var outside = new AustraliaEdition().Cards[^1];
        var (engine, _, _) = Create(chooser0: _ => outside);

        await Assert.ThrowsAsync<InvalidMoveException>(() => engine.PlayAsync());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_RejectsPlayerCountOutsideTwoToFour(int count) =>
        Assert.Throws<GameSetupException>(() => { Create(playerCount: count); });

    [Fact]
    public void Constructor_RejectsDeckTooSmall()
    {
        // Four players need 4 x 3 = 12 cards, the fake edition only has 9.
        var ex = Assert.Throws<GameSetupException>(() => { Create(new FakeEdition(), playerCount: 4); });
        Assert.Contains("deck", ex.Message);
    }

    [Fact]
    public void Constructor_RejectsHandOfOneCard() =>
        Assert.Throws<GameSetupException>(() => { Create(new FakeEdition(handSize: 1)); });

    [Fact]
    public void Constructor_RejectsMisorderedIndexes()
    {
        var state = new GameState();
        state.AddPlayer(new Player(1, "P0", new FakePlayerInteraction()));
        state.AddPlayer(new Player(0, "P1", new FakePlayerInteraction()));
        Assert.Throws<GameSetupException>(() =>
            new GameEngine(new AustraliaEdition(), state, new IdentityShuffler(), new PassNextStrategy()));
    }
}