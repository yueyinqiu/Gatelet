using System.Net.Sockets;
using CliFx.Infrastructure;
using StreamJsonRpc;

namespace Gatelet.Core;

public sealed class GateletClient
{
    private readonly IConsole _console;

    public GateletClient(IConsole console) => _console = console;

    public async Task RunAsync(ConnectInfo info, CancellationToken cancellationToken)
    {
        var endpoint = info switch
        {
            UnixConnectInfo u => (GateletEndpoint)new UnixGateletEndpoint(u.Path),
            TcpConnectInfo t => new TcpGateletEndpoint(t.Host, t.Port),
            _ => throw new InvalidOperationException($"unsupported connect info: {info.GetType().Name}"),
        };

        using var socket = endpoint.Connect();
        using var stream = new NetworkStream(socket, ownsSocket: true);

        var callback = new ConsoleCallback(_console);
        using var rpc = JsonRpc.Attach(stream, callback);
        var gatelet = rpc.Attach<IGatelet>();

        var exitCode = await gatelet.RunAsync(info.Secret, cancellationToken);

        _console.Error.Write(callback.StderrEndsWithNewline ? $"{exitCode}\n" : $"\n{exitCode}\n");
    }
}