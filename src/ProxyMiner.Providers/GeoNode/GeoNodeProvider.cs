using System.Text.Json;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Providers.GeoNode;

/// <summary>
///     Retrieves the proxy list from a proxylist.geonode.com
/// </summary>
public sealed class GeoNodeProvider : IProxyProvider
{
    public Task<ProxyProviderResult> GetProxies(CancellationToken token)
    {
        return Task.Run(async () =>
            {
                var proxies = new List<ProxyDto>();

                var page = 1;
                do
                {
                    token.ThrowIfCancellationRequested();
                    
                    var pageUrl = _url.Replace("%page%", page.ToString());
                    var html = await LoadContent(pageUrl, token);

                    var data = JsonSerializer.Deserialize<PageDto>(
                        html,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (data?.Data == null || data.Total == null)
                        break;

                    proxies.AddRange(data.Data);

                    if (Limit * page > data.Total)
                        break;

                    page++;
                } while (true);

                return ProxyProviderResult.Ok(proxies
                    .Where(r => (r.PortAsNumber != null 
                        && !string.IsNullOrEmpty(r.Ip)
                        && r.Type == GeoNodeProxyTypes.Elite) || r.Type == GeoNodeProxyTypes.Anonymous)
                    .Select(r => Proxy.Factory.TryMakeProxy(
                            r.ProxyType!.Value, r.Ip!, r.PortAsNumber!.Value))
                    .Where(tuple => tuple.Item1 != null)
                    .Select(tuple => tuple.Item1!));

                async Task<string> LoadContent(string url, CancellationToken ct)
                {
                    using var client = new HttpClient();
                    return await client.GetStringAsync(url, ct);
                }
            },
            token);
    }

    private const int Limit = 500;

    private readonly string _url =
        $"https://proxylist.geonode.com/api/proxy-list?limit={Limit}&page=%page%&sort_by=lastChecked&sort_type=desc";
}