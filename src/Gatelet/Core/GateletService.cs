using System.Security.Cryptography;
using System.Text;
using CliWrap;

namespace Gatelet.Core;

/// <summary>Runs the fixed command once per invocation, streaming raw output to the client.</summary>
public sealed class GateletService(string command, string secret, string shell) : IGatelet
{
    private readonly byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
    private IGateletCallback? callback;

    public void SetCallback(IGateletCallback callback) => this.callback = callback;

    public async Task<int> RunAsync(string secret, CancellationToken cancellationToken)
    {
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(secret), secretBytes))
            throw new UnauthorizedAccessException("invalid token");

        if (callback is null)
            throw new InvalidOperationException("client callback is not available");

        var stdOutPipe = PipeTarget.Create((stream, pipeCancellationToken) =>
            PumpAsync(stream, callback.OnStdout, pipeCancellationToken));
        var stdErrPipe = PipeTarget.Create((stream, pipeCancellationToken) =>
            PumpAsync(stream, callback.OnStderr, pipeCancellationToken));

        var shellParts = shell.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = await Cli.Wrap(shellParts[0])
            .WithArguments(shellParts.Skip(1).Append(command))
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(stdOutPipe)
            .WithStandardErrorPipe(stdErrPipe)
            .ExecuteAsync(cancellationToken);

        return result.ExitCode;
    }

    private static async Task PumpAsync(Stream stream, Func<string, Task> write, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream);
        var buffer = new char[8192];
        int read;
        while ((read = await reader.ReadAsync(buffer, cancellationToken)) > 0)
            await write(new string(buffer, 0, read));
    }
}