using Boomerang.Application.Players;
using Boomerang.Domain.Scoring;

namespace Boomerang.Application.Models;

public sealed class Player
{
    public Player(int index, string name, IPlayerInteraction interaction)
        : this(index, name, interaction, (IPlayerNotifications)interaction)
    {
    }

    public Player(int index, string name, IPlayerDecisions decisions, IPlayerNotifications? notifications = null)
    {
        Index = index;
        Name = name;
        Decisions = decisions;
        Notifications = notifications ?? NullPlayerNotifications.Instance;
    }

    /// <summary>Position in the seating order; must equal the position in the list given to the engine.</summary>
    public int Index { get; }
    public string Name { get; }
    public IPlayerDecisions Decisions { get; }
    public IPlayerNotifications Notifications { get; }
    public ScoreSheet Sheet { get; } = new();
}
