using System.Text.Json;

namespace ProxyMiner.Producers.ProxyProviders;

/// <summary>
///     Retrieves the proxy list from a proxylist.geonode.com
/// </summary>
public sealed class GeoNodeProvider : BaseProvider
{
    public override Task<IEnumerable<Proxy>> GetProxies(CancellationToken token)
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

                return proxies
                    .Where(r => (r.PortAsNumber != null 
                        && !string.IsNullOrEmpty(r.Ip)
                        && r.Type == FreeProxyTypes.Elite) || r.Type == FreeProxyTypes.Anonymous)
                    .Select(r =>
                        new Proxy(
                            r.ProxyType!.Value,
                            r.Ip!,
                            r.PortAsNumber!.Value));

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

    private class PageDto
    {
        public List<ProxyDto>? Data { get; set; }
        public int? Total { get; set; }
        public int? Page { get; set; }
        public int? Limit { get; set; }
    }

    private class ProxyDto
    {
        public string? Ip { get; set; }
        public string? AnonymityLevel { get; set; }
        public string? Port { get; set; }
        public List<string>? Protocols { get; set; }

        public ProxyType? ProxyType => ParseProxyType(Protocols?.FirstOrDefault());
        public FreeProxyTypes? Type => ParseType(AnonymityLevel);
        public int? PortAsNumber => ParsePort(Port);

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

        private static ProxyType? ParseProxyType(string? str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (str.Equals(Socks4, StringComparison.OrdinalIgnoreCase))
                return ProxyMiner.ProxyType.Socks4;

            if (str.Equals(Socks5, StringComparison.OrdinalIgnoreCase))
                return ProxyMiner.ProxyType.Socks5;

            return str.Equals(Http, StringComparison.OrdinalIgnoreCase)
                ? ProxyMiner.ProxyType.Http
                : null;
        }
    
        private const string Http = "http";
        private const string Socks4 = "socks4";
        private const string Socks5 = "socks5";

        private const string AnonymousProxyTag = "anonymous";
        private const string EliteProxyTag = "elite";
    }

    private enum FreeProxyTypes
    {
        Anonymous,
        Elite,
        Transparent
    }
}