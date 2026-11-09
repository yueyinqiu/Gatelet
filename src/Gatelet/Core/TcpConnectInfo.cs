namespace Gatelet.Core;

public sealed class TcpConnectInfo : ConnectInfo
{
    public required string Host { get; init; }
    public required int Port { get; init; }
}