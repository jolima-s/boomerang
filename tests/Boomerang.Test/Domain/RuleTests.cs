using Boomerang.Domain.Editions.Shared.DefaultRules;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;

namespace Boomerang.Test.Domain;

internal static class RuleTestHelpers
{
    public const string Category = "Cat";

    public static RoundContext Context(IEnumerable<string> symbolNames, ScoreSheet? sheet = null)
    {
        var symbols = symbolNames.Select(n => new Symbol(Category, n)).ToList();
        var throwCard = new Card("Throw", "A", "R", 1, symbols);
        var catchCard = new Card("Catch", "B", "R", 2, Array.Empty<Symbol>());
        return new RoundContext(new[] { throwCard, catchCard }, sheet ?? new ScoreSheet());
    }
}

public class ActivitiesRuleTests
{
    private static ActivitiesRule Rule() =>
        new(RuleTestHelpers.Category, new[] { "Swim", "Hike" }, new[] { 2, 4, 6 });

    [Fact]
    public void Evaluate_AlreadyClaimedActivity_IsNotOffered()
    {
        var sheet = new ScoreSheet();
        sheet.Claim(ActivitiesRule.KeyFor("Swim"));

        RuleOutcome outcome = Rule().Evaluate(RuleTestHelpers.Context(new[] { "Swim", "Hike" }, sheet));

        ScoreOption option = Assert.Single(outcome.Options);
        Assert.Equal(ActivitiesRule.KeyFor("Hike"), option.Key);
    }

    [Fact]
    public void Evaluate_AllClaimed_ReturnsFixedZero()
    {
        var sheet = new ScoreSheet();
        sheet.Claim(ActivitiesRule.KeyFor("Swim"));

        RuleOutcome outcome = Rule().Evaluate(RuleTestHelpers.Context(new[] { "Swim" }, sheet));

        Assert.Empty(outcome.Options);
        Assert.Equal(0, outcome.Points);
    }

    [Fact]
    public void Evaluate_UnclaimedActivity_IsOfferedWithPointsForCount()
    {
        RuleOutcome outcome = Rule().Evaluate(RuleTestHelpers.Context(new[] { "Swim", "Swim" }));

        ScoreOption option = Assert.Single(outcome.Options);
        Assert.Equal(4, option.Points);
    }
}

public class CollectionsRuleTests
{
    private static CollectionsRule Rule(int limit = 7) =>
        new(RuleTestHelpers.Category,
            new Dictionary<string, int> { ["A"] = 1, ["B"] = 3, ["C"] = 5 },
            limit);

    [Theory]
    [InlineData(new string[0], 0)]
    [InlineData(new[] { "A" }, 2)]            // 1 -> doubled
    [InlineData(new[] { "B", "B" }, 12)]      // 6 -> doubled
    [InlineData(new[] { "A", "B", "A" }, 10)] // 5 -> doubled
    [InlineData(new[] { "C", "A", "A" }, 14)] // 7 = limit -> doubled
    [InlineData(new[] { "C", "B" }, 8)]       // 8 > limit -> not doubled
    [InlineData(new[] { "Unknown" }, 0)]
    public void Evaluate_AppliesDoublingWithinLimit(string[] symbols, int expected)
    {
        RuleOutcome outcome = Rule().Evaluate(RuleTestHelpers.Context(symbols));

        Assert.Equal(expected, outcome.Points);
    }
}

public class AnimalsRuleTests
{
    private static AnimalsRule Rule() =>
        new(RuleTestHelpers.Category, new Dictionary<string, int> { ["Kangaroo"] = 3, ["Emu"] = 5 });

    [Theory]
    [InlineData(new string[0], 0)]
    [InlineData(new[] { "Kangaroo" }, 0)]
    [InlineData(new[] { "Kangaroo", "Kangaroo" }, 3)]
    [InlineData(new[] { "Kangaroo", "Kangaroo", "Kangaroo" }, 3)]
    [InlineData(new[] { "Kangaroo", "Kangaroo", "Kangaroo", "Kangaroo" }, 6)]
    [InlineData(new[] { "Emu", "Emu" }, 5)]
    [InlineData(new[] { "Kangaroo", "Emu" }, 0)]
    [InlineData(new[] { "Kangaroo", "Emu", "Kangaroo", "Emu" }, 8)]
    [InlineData(new[] { "Wombat", "Wombat" }, 0)]
    public void Evaluate_ScoresPairsPerAnimal(string[] symbols, int expected)
    {
        RuleOutcome outcome = Rule().Evaluate(RuleTestHelpers.Context(symbols));

        Assert.Equal(expected, outcome.Points);
    }
}
