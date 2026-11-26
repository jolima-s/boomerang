namespace Boomerang.Application.Events;

public interface IGameEventPublisher
{
    Task PublishAsync<TEvent>(
        TEvent gameEvent,
        CancellationToken cancellationToken);
}