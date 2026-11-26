using Boomerang.Application.Interfaces;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Boomerang.Infrastructure.Networking;

public sealed class TcpTextChannel : ITextChannel, IDisposable
{
    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private bool _disposed;

    public TcpTextChannel(TcpClient client)
    {
        _client = client;
        var utf8 = new UTF8Encoding(false);
        NetworkStream stream = client.GetStream();
        _reader = new StreamReader(stream, utf8);
        _writer = new StreamWriter(stream, utf8) { AutoFlush = true };
    }

    public Task SendAsync(string text, CancellationToken cancellationToken = default) =>
            WriteAsync(new WireMessage(WireKinds.Message, text), cancellationToken);

    public async Task<string> PromptAsync(string prompt, CancellationToken cancellationToken = default)
    {
        await WriteAsync(new WireMessage(WireKinds.Prompt, prompt), cancellationToken);
        string? line = await _reader.ReadLineAsync(cancellationToken);
        return line ?? throw new IOException("A client disconnected.");
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        try { await WriteAsync(new WireMessage(WireKinds.End, ""), cancellationToken); }
        catch (Exception) { }
    }

    private Task WriteAsync(WireMessage message, CancellationToken cancellationToken) =>
            _writer.WriteLineAsync(JsonSerializer.Serialize(message).AsMemory(), cancellationToken);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try { _writer.Dispose(); } catch (Exception) { }
        try { _reader.Dispose(); } catch (Exception) { }
        _client.Dispose();
    }
}