using CliFx.Infrastructure;

namespace Gatelet.Core;

/// <summary>Streams remote command output to the console.</summary>
public sealed class ConsoleCallback : IGateletCallback
{
    private readonly IConsole _console;

    public ConsoleCallback(IConsole console) => _console = console;

    public Task OnStdout(string text)
    {
        _console.Output.Write(text);
        return Task.CompletedTask;
    }

    public Task OnStderr(string text)
    {
        _console.Error.Write(text);
        return Task.CompletedTask;
    }
}