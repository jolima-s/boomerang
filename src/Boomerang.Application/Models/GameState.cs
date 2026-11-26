using Boomerang.Application.Interfaces;

namespace Boomerang.Application.Models;

public sealed class GameState
{
    private readonly List<Player> _players = new();
    private readonly List<ITextChannel> _channels = new();

    public IReadOnlyList<Player> Players => _players.AsReadOnly();
    public IReadOnlyList<ITextChannel> Channels => _channels.AsReadOnly();

    public void AddPlayer(Player player) => _players.Add(player);
    public void AddChannel(ITextChannel channel) => _channels.Add(channel);
}
