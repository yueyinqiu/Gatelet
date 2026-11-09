using System.Text.Json.Serialization;
using Gatelet.Tcp;
using Gatelet.Unix;

namespace Gatelet.Core;

/// <summary>What the client needs to reach and authenticate against a gate.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Transport")]
[JsonDerivedType(typeof(UnixConnectInfo), "unix")]
[JsonDerivedType(typeof(TcpConnectInfo), "tcp")]
public abstract class ConnectInfo
{
    public required string Secret { get; init; }
}