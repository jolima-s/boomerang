using Boomerang.Application.Interfaces;

namespace Boomerang.Application.Players;

public class PlayerInteractionFactory
{
    private readonly Func<Random> _randomFactory;
    private readonly Func<IPlayerInteraction> _localHuman;
    private readonly Func<ITextChannel, IPlayerInteraction> _remoteHuman;

    public PlayerInteractionFactory(
        Func<Random> randomFactory,
        Func<IPlayerInteraction> localHuman,
        Func<ITextChannel, IPlayerInteraction> remoteHuman)
    {
        _randomFactory = randomFactory;
        _localHuman = localHuman;
        _remoteHuman = remoteHuman;
    }

    public IPlayerDecisions CreateBot() => new BotPlayerInteraction(_randomFactory());

    public IPlayerInteraction CreateLocalHuman() => _localHuman();

    public IPlayerInteraction CreateRemoteHuman(ITextChannel channel) => _remoteHuman(channel);
}
