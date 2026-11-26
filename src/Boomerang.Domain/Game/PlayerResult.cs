using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Game;

public sealed record PlayerResult(
    int PlayerIndex,
    string Name,
    int Total,
    IReadOnlyList<RoundScore> Rounds,
    IReadOnlyDictionary<string, int> CategoryTotals);