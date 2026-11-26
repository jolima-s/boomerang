using Boomerang.Application.Events;
using Boomerang.Application.Interfaces;
using Boomerang.Application.Models;
using Boomerang.Application.Players;

namespace Boomerang.Application.Game;

public sealed class GameCoordinator
{
    private readonly PlayerInteractionFactory _factory;
    private readonly IClientAcceptor _clientAcceptor;
    private readonly IGameEventPublisher _eventPublisher;
    private readonly GameConfiguration _configuration;
    private readonly IGameEngineFactory _engineFactory;

    public GameCoordinator(
        GameConfiguration configuration,
        PlayerInteractionFactory factory,
        IClientAcceptor clientAcceptor,
        IGameEventPublisher eventPublisher,
        IGameEngineFactory engineFactory)
    {
        _configuration = configuration;
        _factory = factory;
        _clientAcceptor = clientAcceptor;
        _eventPublisher = eventPublisher;
        _engineFactory = engineFactory;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var gameState = new GameState();
        gameState.AddPlayer(new Player(0, "Player 1", _factory.CreateLocalHuman()));

        try
        {
            int remoteCount = _configuration.PlayerCount - 1;

            if (remoteCount > 0)
            {
                int index = 1;
                await foreach (ITextChannel channel in _clientAcceptor.AcceptAsync(remoteCount, cancellationToken))
                {
                    gameState.AddChannel(channel);
                    var player = new Player(index, $"Player {index + 1}", _factory.CreateRemoteHuman(channel));
                    gameState.AddPlayer(player);
                    await _eventPublisher.PublishAsync(new PlayerConnectedEvent(player.Index, player.Name), cancellationToken);
                    index++;
                }
            }

            for (int b = _configuration.PlayerCount; b < _configuration.PlayerCount + _configuration.BotsCount; b++)
                gameState.AddPlayer(new Player(b, $"Bot {b + 1}", _factory.CreateBot()));

            var engine = _engineFactory.Create(_configuration.Edition, gameState);
            await engine.PlayAsync(cancellationToken);
        }
        finally
        {
            foreach (ITextChannel channel in gameState.Channels)
            {
                try
                {
                    await channel.CloseAsync(CancellationToken.None);
                }
                catch (Exception)
                {
                    // Cleanup is best-effort so one failing channel does not block the others.
                }

                try
                {
                    (channel as IDisposable)?.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
