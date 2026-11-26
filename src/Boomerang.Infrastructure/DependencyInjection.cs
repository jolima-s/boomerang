using Boomerang.Application.Interfaces;
using Boomerang.Domain.Strategies;
using Boomerang.Infrastructure.Networking;
using Microsoft.Extensions.DependencyInjection;

namespace Boomerang.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IPassingStrategy, PassNextStrategy>();
        services.AddTransient<IShuffler, RandomShuffler>();
        services.AddTransient<IClientAcceptor, GameServer>();

        return services;
    }
}