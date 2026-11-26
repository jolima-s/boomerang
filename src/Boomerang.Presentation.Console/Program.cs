using Boomerang.Application;
using Boomerang.Application.Game;
using Boomerang.Application.Models;
using Boomerang.Application.Players;
using Boomerang.Domain;
using Boomerang.Infrastructure;
using Boomerang.Infrastructure.Networking;
using Boomerang.Presentation.Console;
using Boomerang.Presentation.Console.Communication;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    Console.WriteLine("1) Host a game");
    Console.WriteLine("2) Join a game");
    string mode = Ask("Choose an option", "1");

    if (mode == "1")
    {
        string edition = Ask("Edition", "australia");
        int humans = AskInt("Number of human players", 1);

        while (humans < 1 || humans > 4)
        {
            Console.WriteLine("Number of human players must be between 1 and 4.");
            humans = AskInt("Number of human players", 1);
        }

        int bots = 0;

        if (humans + bots == 1 || humans + bots < 4)
            bots = AskInt("Number of bot players", 0);

        var playerFactory = new PlayerInteractionFactory(
            randomFactory: () => new Random(),
            localHuman: () => new TextPlayerInteraction(new ConsoleTextChannel()),
            remoteHuman: channel => new TextPlayerInteraction(channel));

        var services = new ServiceCollection();
        services.AddSingleton(new GameConfiguration(humans, bots, edition));
        services.AddSingleton(playerFactory);
        services.AddApplication();
        services.AddInfrastructure();
        services.AddConsolePresentation();

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        await serviceProvider.GetRequiredService<GameCoordinator>().RunAsync(cts.Token);
        return 0;
    }
    if (mode == "2")
    {
        string host = Ask("Host address", "localhost");
        await GameClient.RunAsync(host, new ConsoleGameClientView(), cts.Token);
        return 0;
    }
    Console.Error.WriteLine("Unknown option.");
    return 1;
}
catch (OperationCanceledException)
{
    Console.Error.WriteLine("The game was cancelled.");
    return 130;
}
catch (GameSetupException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}
catch (Exception ex) when (ex is IOException or SocketException or InvalidMoveException)
{
    Console.Error.WriteLine($"The game was interrupted: {ex.Message}");
    return 2;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An unexpected error occurred: {ex.Message}");
    return 3;
}

static string Ask(string question, string defaultValue)
{
    Console.Write($"{question} [{defaultValue}]: ");
    string? input = Console.ReadLine();
    return string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
}

static int AskInt(string question, int defaultValue)
{
    while (true)
    {
        if (int.TryParse(Ask(question, defaultValue.ToString()), out int value) && value >= 0)
            return value;
        Console.WriteLine("Please enter a valid number.");
    }
}
