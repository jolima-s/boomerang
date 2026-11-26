namespace Boomerang.Domain.Scoring;

/// <summary>The points one player earned in one round, per category.</summary>
public sealed class RoundScore
{
    public RoundScore(IEnumerable<ScoreEntry> entries) => Entries = entries.ToList().AsReadOnly();

    public IReadOnlyList<ScoreEntry> Entries { get; }
    public int Total => Entries.Sum(e => e.Points);
    public int PointsFor(string category) => Entries.Where(e => e.Category == category).Sum(e => e.Points);
}