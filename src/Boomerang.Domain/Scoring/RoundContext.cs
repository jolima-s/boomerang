using Boomerang.Domain.Models;

namespace Boomerang.Domain.Scoring;

/// <summary>Everything a scoring rule may look at. Rules must not modify anything.</summary>
public sealed class RoundContext
{
    private readonly ScoreSheet _sheet;

    public RoundContext(IReadOnlyList<Card> draft, ScoreSheet sheet)
    {
        if (draft.Count < 2)
            throw new ArgumentException("A round needs at least a Throw card and a Catch card.", nameof(draft));
        Draft = draft.ToList().AsReadOnly();
        _sheet = sheet;
    }

    /// <summary>The player's cards in the order they were obtained: first = Throw, last = Catch.</summary>
    public IReadOnlyList<Card> Draft { get; }
    public Card Throw => Draft[0];
    public Card Catch => Draft[^1];

    public IEnumerable<Symbol> SymbolsOf(string category) => Draft.SelectMany(c => c.SymbolsOf(category));

    /// <summary>Whether the site was visited in an earlier round.</summary>
    public bool HasVisited(string site) => _sheet.HasVisited(site);

    /// <summary>Whether the one-off scoring item was already claimed in an earlier round.</summary>
    public bool IsClaimed(string key) => _sheet.IsClaimed(key);
}
