using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;

namespace Boomerang.Application.Players;

/// <summary>A computer player: picks random cards and always scores the best optional score on offer.</summary>
public sealed class BotPlayerInteraction : IPlayerDecisions
{
    private readonly Random _random;

    // Bots run concurrently, so every bot needs its own Random instance.
    public BotPlayerInteraction(Random random) => _random = random;

    public Task<Card> ChooseCardAsync(PickContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(context.Hand[_random.Next(context.Hand.Count)]);

    public Task<string?> ChooseOptionalScoreAsync(string category, IReadOnlyList<ScoreOption> options, CancellationToken cancellationToken = default)
    {
        ScoreOption best = options.OrderByDescending(o => o.Points).First();
        return Task.FromResult<string?>(best.Points > 0 ? best.Key : null);
    }
}
