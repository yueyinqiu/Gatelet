using System.Net;
using System.Net.Sockets;
using Gatelet.Core;

namespace Gatelet.Tcp;

public sealed class TcpGateletEndpoint(string host, int port) : IGateletEndpoint
{
    private int actualPort;

    public Socket Listen()
    {
        var endpoint = new IPEndPoint(Resolve(host), port);
        var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endpoint);

        actualPort = ((IPEndPoint)socket.LocalEndPoint!).Port;
        socket.Listen();
        return socket;
    }

    public Socket Connect()
    {
        var endpoint = new IPEndPoint(Resolve(host), port);
        var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(endpoint);
        return socket;
    }

    public ConnectInfo ToConnectInfo(string secret) => new TcpConnectInfo { Host = host, Port = actualPort, Secret = secret };

    private static IPAddress Resolve(string host) =>
        host is "localhost" ? IPAddress.Loopback : IPAddress.Parse(host);
}