using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Editions.Shared.DefaultRules;

public sealed class ThrowAndCatchRule : IScoringRule
{
    public string Category => ScoreCategories.ThrowAndCatch;

    public RuleOutcome Evaluate(RoundContext context) =>
        RuleOutcome.Fixed(Math.Abs(context.Throw.Number - context.Catch.Number));
}