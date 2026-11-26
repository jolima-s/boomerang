using Boomerang.Application.Events;
using Boomerang.Application.Game;
using Boomerang.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Boomerang.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddTransient<IGameEngineFactory, GameEngineFactory>();
        services.AddTransient<GameCoordinator>();
        services.AddSingleton<IGameEventPublisher, GameEventPublisher>();

        return services;
    }
}