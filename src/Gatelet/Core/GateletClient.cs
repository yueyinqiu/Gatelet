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
        var endpoint = info.Transport switch
        {
            "unix" => (GateletEndpoint)new UnixGateletEndpoint(info.Path!),
            "tcp" => new TcpGateletEndpoint(info.Host!, info.Port),
            _ => throw new InvalidOperationException($"unknown transport '{info.Transport}'"),
        };

        using var socket = endpoint.Connect();
        using var stream = new NetworkStream(socket, ownsSocket: true);

        var callback = new ConsoleCallback(_console);
        using var rpc = JsonRpc.Attach(stream, callback);
        var gatelet = rpc.Attach<IGatelet>();

        var exitCode = await gatelet.RunAsync(info.Token, cancellationToken);

        _console.Error.Write(callback.StderrEndsWithNewline ? $"{exitCode}\n" : $"\n{exitCode}\n");
    }
}