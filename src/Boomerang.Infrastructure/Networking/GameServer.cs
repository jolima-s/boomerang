using Boomerang.Application.Interfaces;
using Boomerang.Infrastructure.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Boomerang.Infrastructure.Networking;

public class GameServer : IClientAcceptor
{
    public async IAsyncEnumerable<ITextChannel> AcceptAsync(
        int count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var listener = new TcpListener(IPAddress.Any, NetworkDefaults.Port);
        listener.Start();
        try
        {
            for (int i = 0; i < count; i++)
            {
                TcpClient tcp = await listener.AcceptTcpClientAsync(cancellationToken);
                TcpTextChannel channel;
                try
                {
                    channel = new TcpTextChannel(tcp);
                }
                catch
                {
                    tcp.Dispose();
                    throw;
                }
                yield return channel;
            }
        }
        finally
        {
            listener.Stop();
        }
    }
}
