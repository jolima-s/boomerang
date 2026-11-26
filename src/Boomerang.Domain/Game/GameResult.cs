namespace Boomerang.Domain.Game;

public sealed record GameResult(
    IReadOnlyList<string> Categories,
    IReadOnlyList<PlayerResult> Players,
    IReadOnlyList<PlayerResult> Winners);
