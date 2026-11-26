using Boomerang.Application.Events;
using SysConsole = System.Console;

namespace Boomerang.Presentation.Console.Handlers;

public class ConsolePlayerConnectedHandler
    : IGameEventHandler<PlayerConnectedEvent>
{
    public Task HandleAsync(
        PlayerConnectedEvent gameEvent,
        CancellationToken cancellationToken)
    {
        SysConsole.WriteLine(
            $"\n{gameEvent.PlayerName} connected!");

        return Task.CompletedTask;
    }
}