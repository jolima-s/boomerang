using Boomerang.Application.Events;
using Boomerang.Application.Interfaces;
using Boomerang.Domain.Editions.Australia;
using Boomerang.Domain.Editions.Azeroth;
using Boomerang.Domain.Editions.Shared;
using Boomerang.Presentation.Console.Communication;
using Boomerang.Presentation.Console.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Boomerang.Presentation.Console;

public static class DependencyInjection
{
    public static IServiceCollection AddConsolePresentation(
        this IServiceCollection services)
    {
        services.AddTransient<ConsoleTextChannel>();

        services.AddSingleton<IEditionResolver>(_ => new KeyedEditionResolver(
            new Dictionary<string, Func<IGameEdition>>
            {
                ["australia"] = () => new AustraliaEdition(),
                ["azeroth"] = () => new AzerothEdition()
            }));

        services.AddTransient<IGameEventHandler<PlayerConnectedEvent>, ConsolePlayerConnectedHandler>();

        return services;
    }
}