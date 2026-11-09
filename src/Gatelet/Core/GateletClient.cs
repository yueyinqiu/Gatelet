using System.Net.Sockets;
using CliFx.Infrastructure;
using Gatelet.Tcp;
using Gatelet.Unix;
using StreamJsonRpc;

namespace Gatelet.Core;

public sealed class GateletClient(IConsole console)
{
    public async Task RunAsync(ConnectInfo info, CancellationToken cancellationToken)
    {
        IGateletEndpoint endpoint = info switch
        {
            UnixConnectInfo u => new UnixGateletEndpoint(u.Path),
            TcpConnectInfo t => new TcpGateletEndpoint(t.Host, t.Port),
            _ => throw new InvalidOperationException($"unsupported connect info: {info.GetType().Name}"),
        };

        using var socket = endpoint.Connect();
        using var stream = new NetworkStream(socket, ownsSocket: true);

        var callback = new ConsoleCallback(console);
        using var rpc = JsonRpc.Attach(stream, callback);
        var gatelet = rpc.Attach<IGatelet>();

        var exitCode = await gatelet.RunAsync(info.Secret, cancellationToken);

        console.Error.Write($"{Environment.NewLine}{exitCode}{Environment.NewLine}");
    }
}