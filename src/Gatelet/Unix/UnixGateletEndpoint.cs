using System.Net.Sockets;
using System.Security.Cryptography;
using Gatelet.Core;

namespace Gatelet.Unix;

public sealed class UnixGateletEndpoint(string path) : IGateletEndpoint
{
    public static UnixGateletEndpoint RandomIn(string directory) =>
        new(Path.Combine(directory, $"gatelet-{Convert.ToHexString(RandomNumberGenerator.GetBytes(8))}.sock"));

    public Socket Listen()
    {
        DeleteStale(path);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        socket.Bind(new UnixDomainSocketEndPoint(path));
        socket.Listen();
        return socket;
    }

    public Socket Connect()
    {
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        socket.Connect(new UnixDomainSocketEndPoint(path));
        return socket;
    }

    public ConnectInfo ToConnectInfo(string secret) => new UnixConnectInfo { Path = path, Secret = secret };

    private static void DeleteStale(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch { /* ignore */ }
    }
}