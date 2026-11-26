using Boomerang.Application.Interfaces;
using Boomerang.Application.Models;
using Boomerang.Domain.Strategies;

namespace Boomerang.Application.Game;
public sealed class GameEngineFactory : IGameEngineFactory
{
    private readonly IShuffler _shuffler;
    private readonly IPassingStrategy _passing;
    private readonly IEditionResolver _editionResolver;

    public GameEngineFactory(IShuffler shuffler, IPassingStrategy passing, IEditionResolver editionResolver)
    {
        _shuffler = shuffler;
        _passing = passing;
        _editionResolver = editionResolver;
    }

    public GameEngine Create(string edition, GameState gameState) =>
        new GameEngine(_editionResolver.Resolve(edition), gameState, _shuffler, _passing);
}
