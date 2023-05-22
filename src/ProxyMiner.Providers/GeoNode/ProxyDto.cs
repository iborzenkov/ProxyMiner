using ProxyMiner.Core.Models;

namespace ProxyMiner.Providers.GeoNode;
// todo: проверить с sealed и PageDto тоже
internal sealed class ProxyDto
{
    public string? Ip { get; set; }
    public string? AnonymityLevel { get; set; }
    public string? Port { get; set; }
    public List<string>? Protocols { get; set; }

    public ProxyType? ProxyType => ParseProxyType(Protocols?.FirstOrDefault());
    public GeoNodeProxyTypes? Type => ParseType(AnonymityLevel);
    public int? PortAsNumber => ParsePort(Port);

    private static GeoNodeProxyTypes? ParseType(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        if (str.Equals(AnonymousProxyTag, StringComparison.OrdinalIgnoreCase))
            return GeoNodeProxyTypes.Anonymous;

        return str.Equals(EliteProxyTag, StringComparison.OrdinalIgnoreCase)
            ? GeoNodeProxyTypes.Elite
            : GeoNodeProxyTypes.Transparent;
    }

    private static int? ParsePort(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        return int.TryParse(str, out var result)
            ? result
            : null;
    }

    private static ProxyType? ParseProxyType(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        if (str.Equals(Socks4, StringComparison.OrdinalIgnoreCase))
            return ProxyMiner.Core.Models.ProxyType.Socks4;

        if (str.Equals(Socks5, StringComparison.OrdinalIgnoreCase))
            return ProxyMiner.Core.Models.ProxyType.Socks5;

        return str.Equals(Http, StringComparison.OrdinalIgnoreCase)
            ? ProxyMiner.Core.Models.ProxyType.Http
            : null;
    }

    private const string Http = "http";
    private const string Socks4 = "socks4";
    private const string Socks5 = "socks5";

    private const string AnonymousProxyTag = "anonymous";
    private const string EliteProxyTag = "elite";
}