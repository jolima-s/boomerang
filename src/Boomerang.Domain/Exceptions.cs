namespace Boomerang.Domain;

/// <summary>The game cannot be started with the given configuration (e.g. wrong number of players).</summary>
public sealed class GameSetupException : Exception
{
    public GameSetupException(string message) : base(message) { }
}

/// <summary>A player controller returned something that is not allowed by the rules.</summary>
public sealed class InvalidMoveException : Exception
{
    public InvalidMoveException(string message) : base(message) { }
}