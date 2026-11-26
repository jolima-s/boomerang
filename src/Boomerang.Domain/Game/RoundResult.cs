using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Game;

public sealed record RoundResult(
    int Round,
    int TotalRounds,
    IReadOnlyList<Card> Draft,
    RoundScore Score,
    int RunningTotal,
    IReadOnlyCollection<string> ClaimedRegions);