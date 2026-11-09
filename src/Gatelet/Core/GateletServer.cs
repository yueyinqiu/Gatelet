using System.Net.Sockets;
using System.Security.Cryptography;
using CliFx.Infrastructure;
using StreamJsonRpc;

namespace Gatelet.Core;

public sealed class GateletServer
{
    private readonly string _command;
    private readonly string _token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
    private readonly IConsole _console;

    public GateletServer(string command, IConsole console)
    {
        _command = command;
        _console = console;
    }

    public async Task RunAsync(GateletEndpoint endpoint, CancellationToken ct)
    {
        using var socket = endpoint.Listen();
        var blob = ConnectInfoCodec.Encode(endpoint.ToConnectInfo(_token));
        _console.Output.WriteLine($"gate run {blob}");

        while (!ct.IsCancellationRequested)
        {
            Socket client;
            try
            {
                client = await socket.AcceptAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            _ = ServeAsync(client);
        }
    }

    private async Task ServeAsync(Socket client)
    {
        try
        {
            using var stream = new NetworkStream(client, ownsSocket: true);
            var service = new GateletService(_command, _token);
            using var rpc = JsonRpc.Attach(stream, service);
            service.SetCallback(rpc.Attach<IGateletCallback>());
            await rpc.Completion;
        }
        catch
        {
            // A single dropped connection must not kill the gate.
        }
    }
}