namespace Boomerang.Application.Interfaces;

/// <summary>A text conversation with one human: the local console or a remote client.</summary>
public interface ITextChannel
{
    Task SendAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>Shows <paramref name="prompt"/> and waits for one line of input.</summary>
    Task<string> PromptAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>Tells the other side the game is over.</summary>
    Task CloseAsync(CancellationToken cancellationToken = default);
}