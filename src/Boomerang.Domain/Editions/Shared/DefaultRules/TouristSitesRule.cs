using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Editions.Shared.DefaultRules;

public sealed class TouristSitesRule : IScoringRule
{
    public string Category => ScoreCategories.TouristSites;

    public RuleOutcome Evaluate(RoundContext context) =>
        RuleOutcome.Fixed(context.Draft.Select(c => c.Site).Distinct().Count(s => !context.HasVisited(s)));
}