using Boomerang.Application.Interfaces;
using SysConsole = System.Console;

namespace Boomerang.Presentation.Console.Communication;

public sealed class ConsoleTextChannel : ITextChannel
{
    public Task SendAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SysConsole.WriteLine(text);
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public async Task<string> PromptAsync(string prompt, CancellationToken cancellationToken = default)
    {
        SysConsole.Write(prompt);
        string? line = await SysConsole.In.ReadLineAsync(cancellationToken);
        return line ?? throw new EndOfStreamException("Console input was closed.");
    }
}