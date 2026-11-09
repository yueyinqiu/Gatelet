using Gatelet.Core;

namespace Gatelet.Unix;

public sealed class UnixConnectInfo : ConnectInfo
{
    public required string Path { get; init; }
}