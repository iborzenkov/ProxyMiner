namespace ProxyMiner.Providers.FreeProxyList;

/// <summary>
///     Proxy server in the data source view.
/// </summary>
internal sealed class FreeProxyServer
{
    public string? Ip { get; init; }
    public int? Port { get; init; }
    public FreeProxyTypes? Type { get; init; }
}