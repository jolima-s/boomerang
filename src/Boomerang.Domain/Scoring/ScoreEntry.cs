namespace Boomerang.Domain.Scoring;

public sealed record ScoreEntry(string Category, int Points, string? Detail = null);