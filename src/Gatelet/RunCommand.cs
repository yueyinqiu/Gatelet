using System.CommandLine;
using Gatelet.Core;

namespace Gatelet;

public static class RunCommand
{
    public static Command Create()
    {
        var token = new Argument<string>("token");
        token.Description = "The gate token printed by the host.";

        var cmd = new Command("run", "Run the gated command through the gate.");
        cmd.Add(token);

        cmd.SetAction(async (result, cancellationToken) =>
        {
            var value = result.GetValue(token) ?? "";
            var info = ConnectInfoCodec.Decode(value);
            if (info is null)
            {
                Console.Error.WriteLine("invalid gate token");
                return 1;
            }

            await new GateletClient().RunAsync(info, cancellationToken);
            return 0;
        });

        return cmd;
    }
}