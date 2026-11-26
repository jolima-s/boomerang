using Boomerang.Domain.Models;

namespace Boomerang.Domain.Editions.Shared;

public sealed class CardFactory
{
    private readonly string _collectionSymbol;
    private readonly string _animalSymbol;
    private readonly string _activitySymbol;

    public CardFactory(
        string collectionSymbol,
        string animalSymbol,
        string activitySymbol)
    {
        _collectionSymbol = collectionSymbol;
        _animalSymbol = animalSymbol;
        _activitySymbol = activitySymbol;
    }

    public Card Create(CardDefinition definition)
    {
        var symbols = new List<Symbol>();

        if (definition.Collection is not null)
            symbols.Add(new Symbol(_collectionSymbol, definition.Collection));

        if (definition.Animal is not null)
            symbols.Add(new Symbol(_animalSymbol, definition.Animal));

        if (definition.Activity is not null)
            symbols.Add(new Symbol(_activitySymbol, definition.Activity));

        return new Card(
            definition.Name,
            definition.Site,
            definition.Region,
            definition.Number,
            symbols);
    }

    public IReadOnlyList<Card> CreateMany(
        IEnumerable<CardDefinition> definitions)
    {
        return definitions.Select(Create).ToList();
    }
}

public sealed record CardDefinition(
    string Name,
    string Site,
    string Region,
    int Number,
    string? Collection = null,
    string? Animal = null,
    string? Activity = null
);