namespace Boomerang.Domain.Scoring;

/// <summary>One optional way of scoring offered to a player (e.g. "score Swimming for 4 points").</summary>
public sealed record ScoreOption(string Key, string Label, int Points);