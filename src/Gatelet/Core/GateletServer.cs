using System.Net.Sockets;
using System.Security.Cryptography;
using StreamJsonRpc;

namespace Gatelet.Core;

public sealed class GateletServer(IReadOnlyList<string> command)
{
    private readonly string secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

    public async Task RunAsync(IGateletEndpoint endpoint, CancellationToken cancellationToken)
    {
        using var socket = endpoint.Listen();
        var token = ConnectInfoCodec.Encode(endpoint.ToConnectInfo(secret));
        Console.WriteLine($"gate run {token}");

        while (!cancellationToken.IsCancellationRequested)
        {
            Socket client;
            try
            {
                client = await socket.AcceptAsync(cancellationToken);
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
            var service = new GateletService(command, secret);
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