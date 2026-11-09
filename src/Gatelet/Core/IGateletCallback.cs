namespace Gatelet.Core;

/// <summary>Callbacks the host invokes on the client to stream command output.</summary>
public interface IGateletCallback
{
    Task OnStdout(string text);
    Task OnStderr(string text);
}