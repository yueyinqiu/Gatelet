using System.Net.Sockets;

namespace Gatelet.Core;

/// <summary>A listen/connect endpoint for the gate: unix socket or TCP.</summary>
public interface IGateletEndpoint
{
    Socket Listen();
    Socket Connect();
    ConnectInfo ToConnectInfo(string secret);
}