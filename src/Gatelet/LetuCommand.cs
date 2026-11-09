using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using Gatelet.Core;

namespace Gatelet;

[Command("letu", Description = "Let a command through via a Unix socket (host side).")]
public partial class LetuCommand : ICommand
{
    [CommandParameter(0, Description = "Command to run on each trigger.")]
    public required string Command { get; set; }

    [CommandOption("directory", Description = "Directory for the socket.")]
    public string SocketDirectory { get; set; } = Directory.GetCurrentDirectory();

    [CommandOption("path", Description = "Socket name (relative to --directory) or absolute path; omit for a random name.")]
    public string? SocketPath { get; set; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var endpoint = SocketPath is null
            ? UnixGateletEndpoint.RandomIn(SocketDirectory)
            : new UnixGateletEndpoint(Path.Combine(SocketDirectory, SocketPath));

        await new GateletServer(Command, console).RunAsync(endpoint, console.RegisterCancellationHandler());
    }
}