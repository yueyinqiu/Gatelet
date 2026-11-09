using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using Gatelet.Core;

namespace Gatelet;

[Command("run", Description = "Run the gated command through the gate (sandbox side).")]
public partial class RunCommand : ICommand
{
    [CommandParameter(0, Description = "The gate token printed by the host.")]
    public required string Token { get; set; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var info = ConnectInfoCodec.Decode(Token);
        await new GateletClient(console).RunAsync(info, console.RegisterCancellationHandler());
    }
}