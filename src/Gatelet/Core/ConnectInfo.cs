namespace Gatelet.Core;

/// <summary>What the client needs to reach and authenticate against a gate.</summary>
public sealed record ConnectInfo(string Transport, string? Path, string? Host, int Port, string Secret);