using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using Gatelet.Core;
using Gatelet.Tcp;

namespace Gatelet;

[Command("lett", Description = "Let a command through via TCP (host side).")]
public partial class LettCommand : ICommand
{
    [CommandParameter(0, Description = "Command to run on each trigger.")]
    public required string Command { get; set; }

    [CommandOption("host", 'H', Description = "Host to listen on.", EnvironmentVariable = "GATELET_LETT_HOST")]
    public string Host { get; set; } = "127.0.0.1";

    [CommandOption("port", 'p', Description = "Port to listen on; 0 = random.", EnvironmentVariable = "GATELET_LETT_PORT")]
    public int Port { get; set; } = 0;

    [CommandOption("shell", 's', Description = "Shell to run the command with; the command is appended as the last argument.", EnvironmentVariable = "GATELET_SHELL")]
    public string Shell { get; set; } = "sh -c";

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var endpoint = new TcpGateletEndpoint(Host, Port);
        await new GateletServer(Command, Shell, console).RunAsync(endpoint, console.RegisterCancellationHandler());
    }
}