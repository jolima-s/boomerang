namespace Boomerang.Application.Events;

public interface IGameEventHandler<TEvent>
{
    Task HandleAsync(
        TEvent gameEvent,
        CancellationToken cancellationToken);
}