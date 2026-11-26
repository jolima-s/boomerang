using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Editions.Shared.DefaultRules;

public sealed class AnimalsRule : IScoringRule
{
    private readonly string _symbolCategory;
    private readonly IReadOnlyDictionary<string, int> _values;

    public AnimalsRule(string symbolCategory, IReadOnlyDictionary<string, int> values)
    {
        _symbolCategory = symbolCategory;
        _values = values;
    }

    public string Category => ScoreCategories.Animals;

    public RuleOutcome Evaluate(RoundContext context)
    {
        int points = context.SymbolsOf(_symbolCategory)
            .GroupBy(s => s.Name)
            .Sum(g => _values.TryGetValue(g.Key, out int v) ? (g.Count() / 2) * v : 0);
        return RuleOutcome.Fixed(points);
    }
}