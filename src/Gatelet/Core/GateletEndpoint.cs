using System.Net.Sockets;

namespace Gatelet.Core;

/// <summary>A listen/connect endpoint for the gate: unix socket or TCP.</summary>
public abstract class GateletEndpoint
{
    public abstract Socket Listen();
    public abstract Socket Connect();
    public abstract ConnectInfo ToConnectInfo(string token);
}