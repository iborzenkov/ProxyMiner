using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Providers.FreeProxyList;

/// <summary>
///     Retrieves the proxy list from a https://free-proxy-list.net/
/// </summary>
public sealed class FreeProxyListProvider : IProxyProvider
{
    public Task<ProxyProviderResult> GetProxies(CancellationToken token)
    {
        return Task.Run(async () =>
            {
                token.ThrowIfCancellationRequested();

                using var client = new HttpClient();
                var html = await client.GetStringAsync(Url, token);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var rows = doc.DocumentNode.QuerySelectorAll("table tbody tr")
                    .Select(s => new FreeProxyServer
                    {
                        Ip = s.QuerySelector("td:nth-child(1)")?.InnerHtml,
                        Port = ParsePort(s.QuerySelector("td:nth-child(2)")?.InnerHtml),
                        Type = ParseType(s.QuerySelector("td:nth-child(5)")?.InnerHtml)
                    })
                    .Where(s => s is { Port: not null, Ip: not null });

                return ProxyProviderResult.Ok(rows
                    .Where(r => r.Type is FreeProxyTypes.Elite or FreeProxyTypes.Anonymous)
                    .Select(r => Proxy.Factory.TryMakeProxy(ProxyType.Http, r.Ip!, r.Port!.Value))
                    .Where(tuple => tuple.Item1 != null)
                    .Select(tuple => tuple.Item1!));
            },
            token);
    }

    private static FreeProxyTypes? ParseType(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        if (str.Equals(AnonymousProxyTag, StringComparison.OrdinalIgnoreCase))
            return FreeProxyTypes.Anonymous;

        return str.Equals(EliteProxyTag, StringComparison.OrdinalIgnoreCase)
            ? FreeProxyTypes.Elite
            : FreeProxyTypes.Transparent;
    }

    private static int? ParsePort(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        return int.TryParse(str, out var result)
            ? result
            : null;
    }

    private const string AnonymousProxyTag = "anonymous";
    private const string EliteProxyTag = "elite proxy";

    private const string Url = "https://free-proxy-list.net/";
}