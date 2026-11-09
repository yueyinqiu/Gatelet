namespace Gatelet.Core;

public sealed class UnixConnectInfo : ConnectInfo
{
    public required string Path { get; init; }
}