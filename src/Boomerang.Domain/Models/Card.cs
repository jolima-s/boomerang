namespace Boomerang.Domain.Models;

public class Card : IEquatable<Card>
{
    public Card(string name, string site, string region, int number, IEnumerable<Symbol> symbols)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(site);
        ArgumentException.ThrowIfNullOrWhiteSpace(region);
        Name = name;
        Site = site;
        Region = region;
        Number = number;
        Symbols = symbols.ToList().AsReadOnly();
    }

    public string Name { get; }
    /// <summary>The map letter of the site (A-Z, '@' or '#').</summary>
    public string Site { get; }
    public string Region { get; }
    /// <summary>The number used for Throw &amp; Catch.</summary>
    public int Number { get; }
    public IReadOnlyList<Symbol> Symbols { get; }

    public IEnumerable<Symbol> SymbolsOf(string category) => Symbols.Where(s => s.Category == category);

    public bool Equals(Card? other) => other is not null && Site == other.Site;
    public override bool Equals(object? obj) => Equals(obj as Card);
    public override int GetHashCode() => Site.GetHashCode();
    public override string ToString() => $"{Name} [{Site}]";
}
