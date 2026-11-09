# Gatelet

A controlled sally port from your sandbox to the host: let a *fixed* command
through a gate on the host, then run it from inside the sandbox over a Unix
domain socket or TCP.

## Usage

On the host, let a command through. `letu` uses a Unix socket, `lett` uses TCP:

```sh
# Unix socket: random name in the current directory
gate letu 'make build'

# Unix socket: random name in a specific directory
gate letu 'make build' --directory /home/you/shared

# Unix socket: explicit path
gate letu 'make build' --path /home/you/shared/build.sock

# TCP: random port on 127.0.0.1
gate lett 'make build'

# TCP: fixed port
gate lett 'make build' --port 8080
```

This prints a single trigger command:

```
gate run eyJUcmFuc3BvcnQiOiJ1bml4Ii…
```

From inside the sandbox, paste and run it to trigger the command (its stdout /
stderr stream back to you).

## Options

`gate letu …`:

- `--directory` — directory for the socket (default: current directory)
- `--path` — socket name relative to `--directory`, or an absolute path
  (default: a random name)

`gate lett …`:

- `--host` — host to listen on (default: `127.0.0.1`)
- `--port` — port to listen on, `0` = random (default: `0`)

## Output and exit code

- the gated command's stdout goes to `gate run`'s stdout;
- its stderr goes to `gate run`'s stderr;
- the gated command's exit code is written as a bare number on the final line
  of `gate run`'s stderr;
- `gate run`'s own exit code is `0` only when it reached the gate and the
  command ran; any non-zero means `gate` itself failed (e.g. connect/token).

## Development

```sh
nix develop
dotnet build src/Gatelet/Gatelet.csproj
```

## License

MIT