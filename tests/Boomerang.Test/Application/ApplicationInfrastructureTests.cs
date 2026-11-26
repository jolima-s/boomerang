using Boomerang.Application.Events;
using Boomerang.Application.Game;
using Boomerang.Application.Interfaces;
using Boomerang.Application.Models;
using Boomerang.Application.Players;
using Boomerang.Domain;
using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Strategies;
using Boomerang.Test.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace Boomerang.Test.Application;

public class GameConfigurationTests
{
    [Theory]
    [InlineData(1, -1)]
    [InlineData(2, -5)]
    public void Constructor_NegativeBots_Throws(int players, int bots) =>
        Assert.Throws<GameSetupException>(() => new GameConfiguration(players, bots, "x"));

    [Fact]
    public void Constructor_NoHumans_Throws() =>
        Assert.Throws<GameSetupException>(() => new GameConfiguration(0, 2, "x"));

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 3)]
    [InlineData(5, 0)]
    public void Constructor_TotalsAreLeftToTheEdition(int players, int bots) =>
        Assert.Equal(players + bots, new GameConfiguration(players, bots, "x").PlayerCount + bots);

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(4, 0)]
    public void Constructor_ValidCounts_Succeeds(int players, int bots)
    {
        var config = new GameConfiguration(players, bots, "x");

        Assert.Equal(players, config.PlayerCount);
        Assert.Equal(bots, config.BotsCount);
    }
}

public class GameCoordinatorTests
{
    private sealed class FakeChannel : ITextChannel, IDisposable
    {
        public bool Closed { get; private set; }
        public bool Disposed { get; private set; }
        public Task SendAsync(string text, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<string> PromptAsync(string prompt, CancellationToken cancellationToken = default) => Task.FromResult("");
        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            Closed = true;
            return Task.CompletedTask;
        }
        public void Dispose() => Disposed = true;
    }

    private sealed class FakeAcceptor : IClientAcceptor
    {
        private readonly IReadOnlyList<ITextChannel> _channels;
        public int RequestedCount { get; private set; }
        public FakeAcceptor(IReadOnlyList<ITextChannel> channels) => _channels = channels;

        public async IAsyncEnumerable<ITextChannel> AcceptAsync(
            int count,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            RequestedCount = count;
            foreach (ITextChannel channel in _channels.Take(count))
            {
                await Task.Yield();
                yield return channel;
            }
        }
    }

    private sealed class RecordingPublisher : IGameEventPublisher
    {
        public List<object?> Events { get; } = new();
        public Task PublishAsync<TEvent>(TEvent gameEvent, CancellationToken cancellationToken)
        {
            Events.Add(gameEvent);
            return Task.CompletedTask;
        }
    }

    private sealed class DelegateEngineFactory : IGameEngineFactory
    {
        private readonly Func<string, GameState, GameEngine> _create;
        public DelegateEngineFactory(Func<string, GameState, GameEngine> create) => _create = create;
        public GameEngine Create(string edition, GameState gameState) => _create(edition, gameState);
    }

    private static PlayerInteractionFactory Factory() =>
        new(() => new Random(1), () => new FakePlayerInteraction(), _ => new FakePlayerInteraction());

    [Fact]
    public async Task RunAsync_AcceptsExpectedNumberOfRemoteConnections()
    {
        var channels = new[] { new FakeChannel(), new FakeChannel() };
        var acceptor = new FakeAcceptor(channels);
        var publisher = new RecordingPublisher();
        GameState? captured = null;
        var engineFactory = new DelegateEngineFactory((edition, state) =>
        {
            captured = state;
            return new GameEngine(new Boomerang.Domain.Editions.Australia.AustraliaEdition(), state, new IdentityShuffler(), new PassNextStrategy());
        });
        var coordinator = new GameCoordinator(
            new GameConfiguration(3, 0, "australia"), Factory(), acceptor, publisher, engineFactory);

        await coordinator.RunAsync();

        Assert.Equal(2, acceptor.RequestedCount);
        Assert.NotNull(captured);
        Assert.Equal(3, captured!.Players.Count);
        Assert.Equal(2, captured.Channels.Count);
        Assert.Equal(2, publisher.Events.OfType<PlayerConnectedEvent>().Count());
        Assert.All(channels, c => Assert.True(c.Closed));
    }

    [Fact]
    public async Task RunAsync_EngineThrows_StillClosesAndDisposesChannels()
    {
        var channels = new[] { new FakeChannel(), new FakeChannel() };
        var engineFactory = new DelegateEngineFactory((_, _) => throw new InvalidOperationException("boom"));
        var coordinator = new GameCoordinator(
            new GameConfiguration(3, 0, "australia"), Factory(), new FakeAcceptor(channels),
            new RecordingPublisher(), engineFactory);

        await Assert.ThrowsAsync<InvalidOperationException>(() => coordinator.RunAsync());

        Assert.All(channels, c =>
        {
            Assert.True(c.Closed);
            Assert.True(c.Disposed);
        });
    }
}

public class PlayerInteractionFactoryTests
{
    private sealed class FixedRandom : Random
    {
        public int Value { get; init; }
        public override int Next(int maxValue) => Value;
    }

    [Fact]
    public async Task CreateBot_UsesRandomFromProvider()
    {
        int calls = 0;
        var random = new FixedRandom { Value = 2 };
        var factory = new PlayerInteractionFactory(
            () => { calls++; return random; },
            () => new FakePlayerInteraction(),
            _ => new FakePlayerInteraction());

        IPlayerDecisions bot = factory.CreateBot();

        var hand = new[] { Card("A"), Card("B"), Card("C") };
        var context = new PickContext(1, 1, PickKind.Throw, hand, null, Array.Empty<Card>(), Array.Empty<string>());
        Card chosen = await bot.ChooseCardAsync(context);

        Assert.Equal(1, calls);
        Assert.Same(hand[2], chosen);
    }

    [Fact]
    public void CreateBot_CreatesNewRandomPerBot()
    {
        int calls = 0;
        var factory = new PlayerInteractionFactory(
            () => { calls++; return new Random(1); },
            () => new FakePlayerInteraction(),
            _ => new FakePlayerInteraction());

        factory.CreateBot();
        factory.CreateBot();

        Assert.Equal(2, calls);
    }

    private static Card Card(string site) => new($"Card {site}", site, "R", 1, Array.Empty<Symbol>());
}

public class GameEventPublisherTests
{
    private sealed record TestEvent(int Id);

    private sealed class DelegateHandler : IGameEventHandler<TestEvent>
    {
        private readonly Func<TestEvent, CancellationToken, Task> _handle;
        public DelegateHandler(Func<TestEvent, CancellationToken, Task> handle) => _handle = handle;
        public Task HandleAsync(TestEvent gameEvent, CancellationToken cancellationToken) => _handle(gameEvent, cancellationToken);
    }

    private static GameEventPublisher Publisher(params IGameEventHandler<TestEvent>[] handlers)
    {
        return new GameEventPublisher(new FakeScopeFactory(handlers));
    }

    private sealed class FakeScopeFactory : IServiceScopeFactory, IServiceScope, IServiceProvider
    {
        private readonly IGameEventHandler<TestEvent>[] _handlers;
        public FakeScopeFactory(IGameEventHandler<TestEvent>[] handlers) => _handlers = handlers;

        public IServiceScope CreateScope() => this;
        public IServiceProvider ServiceProvider => this;
        public void Dispose() { }

        public object? GetService(Type serviceType) =>
            serviceType == typeof(IEnumerable<IGameEventHandler<TestEvent>>) ? _handlers : null;
    }

    [Fact]
    public async Task PublishAsync_MultipleSubscribers_AllReceiveSameEvent()
    {
        var received = new List<TestEvent>();
        var publisher = Publisher(
            new DelegateHandler((e, _) => { received.Add(e); return Task.CompletedTask; }),
            new DelegateHandler((e, _) => { received.Add(e); return Task.CompletedTask; }));
        var gameEvent = new TestEvent(7);

        await publisher.PublishAsync(gameEvent, CancellationToken.None);

        Assert.Equal(2, received.Count);
        Assert.All(received, e => Assert.Same(gameEvent, e));
    }

    [Fact]
    public async Task PublishAsync_HandlerFails_OtherHandlersStillRun()
    {
        bool secondRan = false;
        var publisher = Publisher(
            new DelegateHandler((_, _) => throw new InvalidOperationException("fail")),
            new DelegateHandler((_, _) => { secondRan = true; return Task.CompletedTask; }));

        var ex = await Assert.ThrowsAsync<AggregateException>(
            () => publisher.PublishAsync(new TestEvent(1), CancellationToken.None));

        Assert.True(secondRan);
        Assert.IsType<InvalidOperationException>(Assert.Single(ex.InnerExceptions));
    }

    [Fact]
    public async Task PublishAsync_Cancellation_Propagates()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        bool secondRan = false;
        CancellationToken seen = default;
        var publisher = Publisher(
            new DelegateHandler((_, ct) => { seen = ct; ct.ThrowIfCancellationRequested(); return Task.CompletedTask; }),
            new DelegateHandler((_, _) => { secondRan = true; return Task.CompletedTask; }));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => publisher.PublishAsync(new TestEvent(1), cts.Token));

        Assert.Equal(cts.Token, seen);
        Assert.False(secondRan);
    }
}
