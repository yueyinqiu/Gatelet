using Gatelet.Core;

namespace Gatelet.Tcp;

public sealed class TcpConnectInfo : ConnectInfo
{
    public required string Host { get; init; }
    public required int Port { get; init; }
}