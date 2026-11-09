using System.Net.Sockets;
using Gatelet.Tcp;
using Gatelet.Unix;
using StreamJsonRpc;

namespace Gatelet.Core;

public sealed class GateletClient
{
    public async Task RunAsync(ConnectInfo info, CancellationToken cancellationToken)
    {
        var endpoint = info switch
        {
            UnixConnectInfo u => (IGateletEndpoint)new UnixGateletEndpoint(u.Path),
            TcpConnectInfo t => new TcpGateletEndpoint(t.Host, t.Port),
            _ => throw new InvalidOperationException($"unsupported connect info: {info.GetType().Name}"),
        };

        using var socket = endpoint.Connect();
        using var stream = new NetworkStream(socket, ownsSocket: true);

        var callback = new ConsoleCallback();
        using var rpc = JsonRpc.Attach(stream, callback);
        var gatelet = rpc.Attach<IGatelet>();

        var exitCode = await gatelet.RunAsync(info.Secret, cancellationToken);

        Console.Error.Write($"{Environment.NewLine}{exitCode}{Environment.NewLine}");
    }
}