namespace Boomerang.Application.Interfaces;

public interface IClientAcceptor
{
    /// <summary>
    /// Accepts remote connections one at a time and yields a channel for each, without creating players.
    /// </summary>
    IAsyncEnumerable<ITextChannel> AcceptAsync(int count, CancellationToken cancellationToken = default);
}
