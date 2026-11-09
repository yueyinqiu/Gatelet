using System.Net;
using System.Net.Sockets;

namespace Gatelet.Core;

public sealed class TcpGateletEndpoint : GateletEndpoint
{
    private readonly string _host;
    private readonly int _port;
    private int _actualPort;

    public TcpGateletEndpoint(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public override Socket Listen()
    {
        var endpoint = new IPEndPoint(Resolve(_host), _port);
        var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endpoint);

        _actualPort = ((IPEndPoint)socket.LocalEndPoint!).Port;
        socket.Listen();
        return socket;
    }

    public override Socket Connect()
    {
        var endpoint = new IPEndPoint(Resolve(_host), _port);
        var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(endpoint);
        return socket;
    }

    public override ConnectInfo ToConnectInfo(string secret) => new("tcp", null, _host, _actualPort, secret);

    private static IPAddress Resolve(string host) =>
        host is "localhost" ? IPAddress.Loopback : IPAddress.Parse(host);
}