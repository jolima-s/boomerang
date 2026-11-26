using Boomerang.Domain.Game;

namespace Boomerang.Application.Players;

/// <summary>Notification sink for players that do not need to be told anything (e.g. bots).</summary>
public sealed class NullPlayerNotifications : IPlayerNotifications
{
    public static NullPlayerNotifications Instance { get; } = new();

    private NullPlayerNotifications() { }

    public Task OnCardsRevealedAsync(IReadOnlyList<PlayerReveal> reveals, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task OnRoundScoredAsync(RoundResult result, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task OnGameFinishedAsync(GameResult result, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
