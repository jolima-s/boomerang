using Boomerang.Application.Game;
using Boomerang.Application.Models;

namespace Boomerang.Application.Interfaces;

public interface IGameEngineFactory
{
    GameEngine Create(string edition, GameState gameState);
}
