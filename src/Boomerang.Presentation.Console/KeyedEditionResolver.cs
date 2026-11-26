using Boomerang.Application.Interfaces;
using Boomerang.Domain;
using Boomerang.Domain.Editions.Shared;

namespace Boomerang.Presentation.Console;

public sealed class KeyedEditionResolver : IEditionResolver
{
    private readonly IReadOnlyDictionary<string, Func<IGameEdition>> _factories;

    public KeyedEditionResolver(IReadOnlyDictionary<string, Func<IGameEdition>> factories)
    {
        _factories = factories;
    }

    public IGameEdition Resolve(string edition)
    {
        if (!_factories.TryGetValue(edition.ToLowerInvariant(), out Func<IGameEdition>? factory))
            throw new GameSetupException($"Unknown edition '{edition}'.");

        return factory();
    }
}
