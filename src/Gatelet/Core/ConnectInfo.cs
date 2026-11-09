using System.Text.Json.Serialization;

namespace Gatelet.Core;

/// <summary>What the client needs to reach and authenticate against a gate.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "transport")]
[JsonDerivedType(typeof(UnixConnectInfo), "unix")]
[JsonDerivedType(typeof(TcpConnectInfo), "tcp")]
public abstract class ConnectInfo
{
    public required string Secret { get; init; }
}