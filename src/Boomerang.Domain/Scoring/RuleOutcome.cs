namespace Boomerang.Domain.Scoring;

/// <summary>
/// Result of evaluating a rule: either a fixed number of points, or a set of options of which
/// the player may pick at most one.
/// </summary>
public sealed record RuleOutcome(int Points, IReadOnlyList<ScoreOption> Options)
{
    public static RuleOutcome Fixed(int points) => new(points, Array.Empty<ScoreOption>());
    public static RuleOutcome Choice(IReadOnlyList<ScoreOption> options) => new(0, options);
}