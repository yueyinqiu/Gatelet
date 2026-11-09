using System.Security.Cryptography;
using System.Text;
using CliWrap;

namespace Gatelet.Core;

/// <summary>Runs the fixed command once per invocation, streaming raw output to the client.</summary>
public sealed class GateletService : IGatelet
{
    private readonly string _command;
    private readonly byte[] _token;
    private IGateletCallback? _callback;

    public GateletService(string command, string token)
    {
        _command = command;
        _token = Encoding.UTF8.GetBytes(token);
    }

    public void SetCallback(IGateletCallback callback) => _callback = callback;

    public async Task<int> RunAsync(string token, CancellationToken ct)
    {
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(token), _token))
            throw new UnauthorizedAccessException("invalid or missing token");

        if (_callback is null)
            throw new InvalidOperationException("client callback is not available");

        var stdOutPipe = PipeTarget.Create((stream, streamCt) => PumpAsync(stream, _callback.OnStdout, streamCt));
        var stdErrPipe = PipeTarget.Create((stream, streamCt) => PumpAsync(stream, _callback.OnStderr, streamCt));

        var result = await Cli.Wrap("sh")
            .WithArguments(["-c", _command])
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(stdOutPipe)
            .WithStandardErrorPipe(stdErrPipe)
            .ExecuteAsync(ct);

        return result.ExitCode;
    }

    private static async Task PumpAsync(Stream stream, Func<string, Task> write, CancellationToken ct)
    {
        using var reader = new StreamReader(stream);
        var buffer = new char[8192];
        int read;
        while ((read = await reader.ReadAsync(buffer, ct)) > 0)
            await write(new string(buffer, 0, read));
    }
}