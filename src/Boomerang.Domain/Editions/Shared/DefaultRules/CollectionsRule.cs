using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Editions.Shared.DefaultRules;

public sealed class CollectionsRule : IScoringRule
{
    private readonly string _symbolCategory;
    private readonly IReadOnlyDictionary<string, int> _values;
    private readonly int _doubleLimit;

    public CollectionsRule(string symbolCategory, IReadOnlyDictionary<string, int> values, int doubleLimit)
    {
        _symbolCategory = symbolCategory;
        _values = values;
        _doubleLimit = doubleLimit;
    }

    public string Category => ScoreCategories.Collections;

    public RuleOutcome Evaluate(RoundContext context)
    {
        int total = context.SymbolsOf(_symbolCategory).Sum(s => _values.TryGetValue(s.Name, out int v) ? v : 0);
        bool doubled = total >= 1 && total <= _doubleLimit;
        return RuleOutcome.Fixed(doubled ? total * 2 : total);
    }
}