using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Editions.Shared.DefaultRules;

public sealed class ActivitiesRule : IScoringRule
{
    private readonly string _symbolCategory;
    private readonly IReadOnlyList<string> _activities;
    private readonly IReadOnlyList<int> _pointsByCount;

    /// <param name="pointsByCount">Index 0 = points for one icon, index 1 = two icons, and so on.</param>
    public ActivitiesRule(string symbolCategory, IReadOnlyList<string> activities, IReadOnlyList<int> pointsByCount)
    {
        _symbolCategory = symbolCategory;
        _activities = activities;
        _pointsByCount = pointsByCount;
    }

    public string Category => ScoreCategories.Activities;

    public static string KeyFor(string activity) => $"{ScoreCategories.Activities}:{activity}";

    public RuleOutcome Evaluate(RoundContext context)
    {
        var options = new List<ScoreOption>();
        foreach (string activity in _activities)
        {
            string key = KeyFor(activity);
            if (context.IsClaimed(key)) continue;

            int count = context.SymbolsOf(_symbolCategory).Count(s => s.Name == activity);
            if (count == 0) continue;

            int points = _pointsByCount[Math.Min(count, _pointsByCount.Count) - 1];
            options.Add(new ScoreOption(key, $"{activity} (x{count})", points));
        }
        return options.Count == 0 ? RuleOutcome.Fixed(0) : RuleOutcome.Choice(options);
    }
}