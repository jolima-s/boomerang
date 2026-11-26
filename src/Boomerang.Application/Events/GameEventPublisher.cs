using Microsoft.Extensions.DependencyInjection;

namespace Boomerang.Application.Events;

public class GameEventPublisher : IGameEventPublisher
{
    private readonly IServiceScopeFactory _scopeFactory;

    public GameEventPublisher(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task PublishAsync<TEvent>(
        TEvent gameEvent,
        CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var handlers = scope.ServiceProvider
            .GetServices<IGameEventHandler<TEvent>>();

        List<Exception>? failures = null;

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(
                    gameEvent,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                (failures ??= []).Add(ex);
            }
        }

        if (failures is not null)
        {
            throw new AggregateException(
                $"One or more handlers failed for event {typeof(TEvent).Name}.",
                failures);
        }
    }
}