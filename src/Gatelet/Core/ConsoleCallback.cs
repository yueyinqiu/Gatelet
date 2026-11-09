using CliFx.Infrastructure;

namespace Gatelet.Core;

/// <summary>Streams remote command output to the console, tracking stderr's trailing newline.</summary>
public sealed class ConsoleCallback : IGateletCallback
{
    private readonly IConsole _console;

    public bool StderrEndsWithNewline { get; private set; } = true;

    public ConsoleCallback(IConsole console) => _console = console;

    public Task OnStdout(string text)
    {
        _console.Output.Write(text);
        return Task.CompletedTask;
    }

    public Task OnStderr(string text)
    {
        _console.Error.Write(text);
        if (text.Length > 0)
            StderrEndsWithNewline = text[^1] == '\n';
        return Task.CompletedTask;
    }
}