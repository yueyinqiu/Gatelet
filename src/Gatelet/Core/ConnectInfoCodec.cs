using System.Text;
using System.Text.Json;

namespace Gatelet.Core;

/// <summary>Encodes/decodes gate connection info into a single copy-pasteable token.</summary>
public static class ConnectInfoCodec
{
    public static string Encode(ConnectInfo info) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(info)));

    /// <summary>Decodes a token, or returns null when it is not a valid gate token.</summary>
    public static ConnectInfo? Decode(string token)
    {
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            return JsonSerializer.Deserialize<ConnectInfo>(json);
        }
        catch (Exception e) when (e is FormatException or JsonException or NotSupportedException)
        {
            return null;
        }
    }
}