namespace Gatelet.Core;

/// <summary>The RPC surface the host exposes to the sandbox.</summary>
public interface IGatelet
{
    Task<int> RunAsync(string secret, CancellationToken cancellationToken);
}