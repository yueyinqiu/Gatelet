using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using Gatelet.Core;

namespace Gatelet;

[Command("lett", Description = "Let a command through via TCP (host side).")]
public partial class LettCommand : ICommand
{
    [CommandParameter(0, Description = "Command to run on each trigger.")]
    public string? Command { get; set; }

    [CommandOption("host", Description = "Host to listen on; defaults to 127.0.0.1.")]
    public string Host { get; set; } = "127.0.0.1";

    [CommandOption("port", Description = "Port to listen on; 0 = random.")]
    public int Port { get; set; } = 0;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrWhiteSpace(Command))
        {
            console.Error.WriteLine("error: provide a command to let through, e.g. gate lett 'make build'");
            return;
        }

        var endpoint = new TcpGateletEndpoint(Host, Port);
        await new GateletServer(Command, console).RunAsync(endpoint, console.RegisterCancellationHandler());
    }
}