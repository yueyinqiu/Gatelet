using System.Net;
using System.Net.Sockets;
using Gatelet.Core;

namespace Gatelet.Tcp;

public sealed class TcpGateletEndpoint : IGateletEndpoint
{
    private readonly string _host;
    private readonly int _port;
    private int _actualPort;

    public TcpGateletEndpoint(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public Socket Listen()
    {
        var endpoint = new IPEndPoint(Resolve(_host), _port);
        var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endpoint);

        _actualPort = ((IPEndPoint)socket.LocalEndPoint!).Port;
        socket.Listen();
        return socket;
    }

    public Socket Connect()
    {
        var endpoint = new IPEndPoint(Resolve(_host), _port);
        var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(endpoint);
        return socket;
    }

    public ConnectInfo ToConnectInfo(string secret) => new TcpConnectInfo { Host = _host, Port = _actualPort, Secret = secret };

    private static IPAddress Resolve(string host) =>
        host is "localhost" ? IPAddress.Loopback : IPAddress.Parse(host);
}