using System.CommandLine;
using Gatelet.Core;
using Gatelet.Tcp;

namespace Gatelet;

public static class LettCommand
{
    public static Command Create()
    {
        var command = new Argument<string[]>("command");
        command.Description = "Command to run on each trigger.";

        var host = new Option<string?>("--host", ["-H"]);
        host.Description = "Host to listen on.";

        var port = new Option<int?>("--port", ["-p"]);
        port.Description = "Port to listen on; 0 = random.";

        var cmd = new Command("lett", "Let a command through via TCP.");
        cmd.Add(command);
        cmd.Add(host);
        cmd.Add(port);

        cmd.SetAction(async (result, cancellationToken) =>
        {
            var argv = result.GetValue(command) ?? [];
            if (argv.Length == 0)
            {
                Console.Error.WriteLine("error: provide a command to let through");
                return 1;
            }

            var hostName = result.GetValue(host)
                ?? Environment.GetEnvironmentVariable("GATELET_LETT_HOST")
                ?? "127.0.0.1";

            var portNumber = result.GetValue(port)
                ?? int.Parse(Environment.GetEnvironmentVariable("GATELET_LETT_PORT") ?? "0");

            await new GateletServer(argv)
                .RunAsync(new TcpGateletEndpoint(hostName, portNumber), cancellationToken);
            return 0;
        });

        return cmd;
    }
}