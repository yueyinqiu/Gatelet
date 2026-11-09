using System.CommandLine;
using Gatelet.Core;
using Gatelet.Unix;

namespace Gatelet;

public static class LetuCommand
{
    public static Command Create()
    {
        var command = new Argument<string[]>("command");
        command.Description = "Command to run on each trigger.";

        var directory = new Option<string?>("--directory", ["-d"]);
        directory.Description = "Directory for the socket.";

        var path = new Option<string?>("--path", ["-p"]);
        path.Description = "Socket name (relative to --directory) or absolute path; omit for a random name.";

        var cmd = new Command("letu", "Let a command through via a Unix socket.");
        cmd.Add(command);
        cmd.Add(directory);
        cmd.Add(path);

        cmd.SetAction(async (result, cancellationToken) =>
        {
            var argv = result.GetValue(command) ?? [];
            if (argv.Length == 0)
            {
                Console.Error.WriteLine("error: provide a command to let through");
                return 1;
            }

            var dir = result.GetValue(directory)
                ?? Environment.GetEnvironmentVariable("GATELET_LETU_DIRECTORY")
                ?? Directory.GetCurrentDirectory();

            var p = result.GetValue(path) ?? Environment.GetEnvironmentVariable("GATELET_LETU_PATH");

            var endpoint = p is null
                ? UnixGateletEndpoint.RandomIn(dir)
                : new UnixGateletEndpoint(Path.Combine(dir, p));

            await new GateletServer(argv).RunAsync(endpoint, cancellationToken);
            return 0;
        });

        return cmd;
    }
}