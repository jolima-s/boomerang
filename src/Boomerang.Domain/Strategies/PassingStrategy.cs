namespace Boomerang.Domain.Strategies;

public enum PassDirection { Next, Previous }

public static class PassDirectionExtensions
{
    public static PassDirection Opposite(this PassDirection direction) =>
        direction == PassDirection.Next ? PassDirection.Previous : PassDirection.Next;

    /// <summary>The index of the player who receives from <paramref name="index"/>; the ring wraps around.</summary>
    public static int ReceiverOf(this PassDirection direction, int index, int playerCount) =>
        direction == PassDirection.Next
            ? (index + 1) % playerCount
            : (index - 1 + playerCount) % playerCount;
}

/// <summary>
/// Decides in which direction hands are passed during a round. The final (Catch) card always travels in
/// the opposite direction.
/// </summary>
public interface IPassingStrategy
{
    PassDirection DirectionFor(int roundIndex);
}

public sealed class PassNextStrategy : IPassingStrategy
{
    public PassDirection DirectionFor(int roundIndex) => PassDirection.Next;
}

public sealed class AlternatingPassStrategy : IPassingStrategy
{
    public PassDirection DirectionFor(int roundIndex) =>
        roundIndex % 2 == 0 ? PassDirection.Next : PassDirection.Previous;
}