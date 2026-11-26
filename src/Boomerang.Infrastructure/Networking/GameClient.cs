using Boomerang.Infrastructure.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Boomerang.Infrastructure.Networking;

public static class GameClient
{
    public static async Task RunAsync(string host, IGameClientView view, CancellationToken cancellationToken = default)
    {
        using var tcp = new TcpClient();
        await tcp.ConnectAsync(host, NetworkDefaults.Port, cancellationToken);

        var utf8 = new UTF8Encoding(false);
        NetworkStream stream = tcp.GetStream();
        using var reader = new StreamReader(stream, utf8);
        using var writer = new StreamWriter(stream, utf8) { AutoFlush = true };

        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            WireMessage? message;
            try
            {
                message = JsonSerializer.Deserialize<WireMessage>(line);
            }
            catch (JsonException)
            {
                continue;
            }
            if (message is null) continue;

            switch (message.Kind)
            {
                case WireKinds.Message:
                    view.ShowMessage(message.Text ?? "");
                    break;
                case WireKinds.Prompt:
                    await writer.WriteLineAsync((view.ReadResponse(message.Text ?? "") ?? "").AsMemory(), cancellationToken);
                    break;
                case WireKinds.End:
                    return;
            }
        }
        throw new IOException("The connection to the server was lost.");
    }
}