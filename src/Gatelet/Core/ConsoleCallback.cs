namespace Gatelet.Core;

/// <summary>Streams remote command output to the console.</summary>
public sealed class ConsoleCallback : IGateletCallback
{
    public Task OnStdout(string text)
    {
        Console.Out.Write(text);
        return Task.CompletedTask;
    }

    public Task OnStderr(string text)
    {
        Console.Error.Write(text);
        return Task.CompletedTask;
    }
}