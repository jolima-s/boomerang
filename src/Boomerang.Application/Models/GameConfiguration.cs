using Boomerang.Domain;

namespace Boomerang.Application.Models;

public class GameConfiguration
{
    public GameConfiguration(int playerCount, int botsCount, string edition)
    {
        if (playerCount < 1)
            throw new GameSetupException("There must be at least one human player.");
        if (botsCount < 0)
            throw new GameSetupException("Number of bot players cannot be negative.");
        if (string.IsNullOrWhiteSpace(edition))
            throw new GameSetupException("Edition must be specified.");

        PlayerCount = playerCount;
        BotsCount = botsCount;
        Edition = edition;
    }

    public int PlayerCount { get; }
    public int BotsCount { get; }
    public string Edition { get; }
}