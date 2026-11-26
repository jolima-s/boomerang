using Boomerang.Domain.Editions.Shared.DefaultRules;
using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;
using Boomerang.Domain.Strategies;

namespace Boomerang.Test.Domain;

public class PassDirectionTests
{
    [Theory]
    [InlineData(PassDirection.Next, 0, 3, 1)]
    [InlineData(PassDirection.Next, 2, 3, 0)]
    [InlineData(PassDirection.Previous, 0, 3, 2)]
    [InlineData(PassDirection.Previous, 2, 3, 1)]
    public void ReceiverOf_WrapsAround(PassDirection direction, int index, int count, int expected) =>
        Assert.Equal(expected, direction.ReceiverOf(index, count));

    [Fact]
    public void Opposite_Flips()
    {
        Assert.Equal(PassDirection.Previous, PassDirection.Next.Opposite());
        Assert.Equal(PassDirection.Next, PassDirection.Previous.Opposite());
    }

    [Fact]
    public void AlternatingStrategy_AlternatesByRound()
    {
        var s = new AlternatingPassStrategy();
        Assert.Equal(PassDirection.Next, s.DirectionFor(0));
        Assert.Equal(PassDirection.Previous, s.DirectionFor(1));
        Assert.Equal(PassDirection.Next, new PassNextStrategy().DirectionFor(1));
    }
}

public class RandomShufflerTests
{
    [Fact]
    public void Shuffle_KeepsAllItems()
    {
        var items = Enumerable.Range(0, 20).ToList();
        new RandomShuffler(new Random(1)).Shuffle(items);
        Assert.Equal(Enumerable.Range(0, 20), items.OrderBy(i => i));
    }

    [Fact]
    public void Shuffle_SameSeedGivesSameOrder()
    {
        var a = Enumerable.Range(0, 20).ToList();
        var b = Enumerable.Range(0, 20).ToList();
        new RandomShuffler(new Random(7)).Shuffle(a);
        new RandomShuffler(new Random(7)).Shuffle(b);
        Assert.Equal(a, b);
    }
}

public class CardTests
{
    [Fact]
    public void Cards_AreEqualBySite()
    {
        var a = new Card("One", "A", "R", 1, Array.Empty<Symbol>());
        var b = new Card("Other", "A", "R", 5, Array.Empty<Symbol>());
        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Constructor_RejectsBlankName() =>
        Assert.ThrowsAny<ArgumentException>(() => new Card(" ", "A", "R", 1, Array.Empty<Symbol>()));

    [Fact]
    public void SymbolsOf_FiltersByCategory()
    {
        var card = new Card("One", "A", "R", 1, new[] { new Symbol("Animal", "Kangaroo"), new Symbol("Collection", "Leaves") });
        Assert.Single(card.SymbolsOf("Animal"));
    }
}

public class ScoreSheetTests
{
    [Fact]
    public void Total_SumsRounds()
    {
        var sheet = new ScoreSheet();
        sheet.AddRound(new RoundScore(new[] { new ScoreEntry("X", 3), new ScoreEntry("Y", 2) }));
        sheet.AddRound(new RoundScore(new[] { new ScoreEntry("X", 4) }));
        Assert.Equal(9, sheet.Total);
        Assert.Equal(7, sheet.CategoryTotal("X"));
    }

    [Fact]
    public void Claim_TwiceThrows()
    {
        var sheet = new ScoreSheet();
        sheet.Claim("swim");
        Assert.True(sheet.IsClaimed("swim"));
        Assert.Throws<InvalidOperationException>(() => sheet.Claim("swim"));
    }

    [Fact]
    public void MarkVisited_Tracks()
    {
        var sheet = new ScoreSheet();
        sheet.MarkVisited(new[] { "A", "B" });
        Assert.True(sheet.HasVisited("A"));
        Assert.False(sheet.HasVisited("C"));
    }
}

public class RoundContextAndRuleTests
{
    private static Card C(string site, int n) => new("Card " + site, site, "R", n, Array.Empty<Symbol>());

    [Fact]
    public void RoundContext_RequiresTwoCards() =>
        Assert.Throws<ArgumentException>(() => new RoundContext(new[] { C("A", 1) }, new ScoreSheet()));

    [Fact]
    public void ThrowAndCatch_ScoresAbsoluteDifference()
    {
        var ctx = new RoundContext(new[] { C("A", 2), C("B", 9), C("C", 7) }, new ScoreSheet());
        Assert.Equal(5, new ThrowAndCatchRule().Evaluate(ctx).Points);
    }
}

public class RegionBonusTrackerTests
{
    private static readonly Region R = new("R1", new[] { "A", "B" });

    [Fact]
    public void Award_GivesBonusToEveryCompletingPlayerOnce()
    {
        var tracker = new RegionBonusTracker(new[] { R });

        var first = tracker.Award(new[] { new HashSet<string> { "A", "B" }, new HashSet<string> { "A", "B", "C" }, new HashSet<string> { "A" } });
        Assert.Single(first[0]);
        Assert.Single(first[1]);
        Assert.Empty(first[2]);

        var second = tracker.Award(new[] { new HashSet<string> { "A", "B" }, new HashSet<string> { "A" }, new HashSet<string> { "A", "B" } });
        Assert.All(second, list => Assert.Empty(list));
        Assert.Contains("R1", tracker.ClaimedRegions);
    }
}

public class WinnerSelectorTests
{
    private static PlayerResult P(int i, int total, int tie) =>
        new(i, "P" + i, total, Array.Empty<RoundScore>(), new Dictionary<string, int> { ["T"] = tie });

    [Fact]
    public void HighestTotalWins() =>
        Assert.Equal(1, Assert.Single(WinnerSelector.Select(new[] { P(0, 5, 9), P(1, 6, 0) }, "T")).PlayerIndex);

    [Fact]
    public void TieIsBrokenByCategory() =>
        Assert.Equal(0, Assert.Single(WinnerSelector.Select(new[] { P(0, 6, 3), P(1, 6, 2) }, "T")).PlayerIndex);

    [Fact]
    public void FullTieReturnsAll() =>
        Assert.Equal(2, WinnerSelector.Select(new[] { P(0, 6, 3), P(1, 6, 3) }, "T").Count);
}
