using System.Security.Cryptography;
using System.Text;
using CliWrap;

namespace Gatelet.Core;

/// <summary>Runs the fixed command once per invocation, streaming raw output to the client.</summary>
public sealed class GateletService : IGatelet
{
    private readonly string _command;
    private readonly byte[] _secret;
    private IGateletCallback? _callback;

    public GateletService(string command, string secret)
    {
        _command = command;
        _secret = Encoding.UTF8.GetBytes(secret);
    }

    public void SetCallback(IGateletCallback callback) => _callback = callback;

    public async Task<int> RunAsync(string secret, CancellationToken cancellationToken)
    {
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(secret), _secret))
            throw new UnauthorizedAccessException("invalid token");

        if (_callback is null)
            throw new InvalidOperationException("client callback is not available");

        var stdOutPipe = PipeTarget.Create((stream, pipeCancellationToken) =>
            PumpAsync(stream, _callback.OnStdout, pipeCancellationToken));
        var stdErrPipe = PipeTarget.Create((stream, pipeCancellationToken) =>
            PumpAsync(stream, _callback.OnStderr, pipeCancellationToken));

        var result = await Cli.Wrap("sh")
            .WithArguments(["-c", _command])
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