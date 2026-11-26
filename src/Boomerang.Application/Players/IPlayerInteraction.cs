using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;

namespace Boomerang.Application.Players;

/// <summary>The choices the engine asks a player to make.</summary>
public interface IPlayerDecisions
{
    Task<Card> ChooseCardAsync(PickContext context, CancellationToken cancellationToken = default);

    /// <summary>Return the key of the chosen option, or null to score nothing for this category.</summary>
    Task<string?> ChooseOptionalScoreAsync(string category, IReadOnlyList<ScoreOption> options, CancellationToken cancellationToken = default);
}

/// <summary>Information the engine pushes to a player. Fire-and-forget output.</summary>
public interface IPlayerNotifications
{
    Task OnCardsRevealedAsync(IReadOnlyList<PlayerReveal> reveals, CancellationToken cancellationToken = default);
    Task OnRoundScoredAsync(RoundResult result, CancellationToken cancellationToken = default);
    Task OnGameFinishedAsync(GameResult result, CancellationToken cancellationToken = default);
}

/// <summary>
/// A participant that both decides and is notified (console player, network player).
/// The engine never performs input/output itself.
/// </summary>
public interface IPlayerInteraction : IPlayerDecisions, IPlayerNotifications
{
}
