using CliFx.Infrastructure;

namespace Gatelet.Core;

/// <summary>Streams remote command output to the console.</summary>
public sealed class ConsoleCallback(IConsole console) : IGateletCallback
{
    public Task OnStdout(string text)
    {
        console.Output.Write(text);
        return Task.CompletedTask;
    }

    public Task OnStderr(string text)
    {
        console.Error.Write(text);
        return Task.CompletedTask;
    }
}