using System.Text;
using System.Text.Json;

namespace Gatelet.Core;

/// <summary>Encodes/decodes gate connection info into a single copy-pasteable token.</summary>
public static class ConnectInfoCodec
{
    public static string Encode(ConnectInfo info)
    {
        var json = JsonSerializer.Serialize(info);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static ConnectInfo Decode(string token)
    {
        var s = token.Replace('-', '+').Replace('_', '/');
        var pad = (s.Length % 4) switch { 2 => "==", 3 => "=", _ => "" };

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(s + pad));
        return JsonSerializer.Deserialize<ConnectInfo>(json)
            ?? throw new InvalidDataException("invalid gate run token");
    }
}