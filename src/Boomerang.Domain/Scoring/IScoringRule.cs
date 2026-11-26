namespace Boomerang.Domain.Scoring;

/// <summary>
/// One way of scoring at the end of a round. New editions add new rules (or replace existing ones)
/// without touching the game engine.
/// </summary>
public interface IScoringRule
{
    string Category { get; }
    RuleOutcome Evaluate(RoundContext context);
}