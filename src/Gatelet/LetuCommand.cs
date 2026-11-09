using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using Gatelet.Core;

namespace Gatelet;

[Command("letu", Description = "Let a command through via a Unix socket (host side).")]
public partial class LetuCommand : ICommand
{
    [CommandParameter(0, Description = "Command to run on each trigger.")]
    public string? Command { get; set; }

    [CommandOption("directory", Description = "Directory for the socket; defaults to the current directory.")]
    public string SocketDir { get; set; } = Directory.GetCurrentDirectory();

    [CommandOption("path", Description = "Socket name (relative to --directory) or absolute path; default: random.")]
    public string? SocketPath { get; set; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(Command))
        {
            console.Error.WriteLine("error: provide a command to let through, e.g. gate letu 'make build'");
            return;
        }

        var endpoint = SocketPath is null
            ? UnixGateletEndpoint.RandomIn(SocketDir)
            : new UnixGateletEndpoint(Path.Combine(SocketDir, SocketPath));

        await new GateletServer(Command, console).RunAsync(endpoint, console.RegisterCancellationHandler());
    }
}