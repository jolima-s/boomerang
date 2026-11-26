namespace Boomerang.Domain.Models;

/// <summary>
/// A symbol printed on a card. The category is free text ("Animal", "Collection", ...) so a new edition
/// can introduce new kinds of symbols without changing this type.
/// </summary>
public sealed record Symbol(string Category, string Name);
