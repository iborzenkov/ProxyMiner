namespace ProxyMiner.Producers.ProxyProviders;

/// <summary>
///     Retrieves the proxy list from a CSV-file.
/// </summary>
public sealed class CsvFileProvider : BaseProvider
{
    public CsvFileProvider(string filename, CsvFileSettings settings)
    {
        _filename = filename;
        _settings = settings;
    }

    public override Task<IEnumerable<Proxy>> GetProxies(CancellationToken token)
    {
        return File.Exists(_filename)
            ? Task.Run<IEnumerable<Proxy>>(() =>
                {
                    var result = new List<Proxy>();
                    foreach (var line in File.ReadLines(_filename))
                    {
                        token.ThrowIfCancellationRequested();

                        try
                        {
                            var parts = line.Split(";");

                            var type = GetProxyType(parts[_settings.TypeColumn]);
                            var host = parts[_settings.HostColumn];
                            var port = int.Parse(parts[_settings.PortColumn]);
                            var username = _settings.UsernameColumn == null 
                                ? null 
                                : parts[_settings.UsernameColumn.Value];
                            var password = _settings.PasswordColumn == null
                                ? null
                                : parts[_settings.PasswordColumn.Value];

                            result.Add(new Proxy(type, host, port, username, password));
                        }
                        catch
                        {
                            // skip the whole line
                        }
                    }

                    return result;
                },
                token)
            : Task.FromResult(Enumerable.Empty<Proxy>());
    }

    private static ProxyType GetProxyType(string proxyTypeAsString)
    {
        if (proxyTypeAsString.Equals(Http, StringComparison.OrdinalIgnoreCase))
            return ProxyType.Http;

        if (proxyTypeAsString.Equals(Socks4, StringComparison.OrdinalIgnoreCase))
            return ProxyType.Socks4;

        if (proxyTypeAsString.Equals(Socks5, StringComparison.OrdinalIgnoreCase))
            return ProxyType.Socks5;

        throw new ArgumentOutOfRangeException($"Proxy type in CSV ({proxyTypeAsString}) is unknown");
    }

    private const string Http = "http";
    private const string Socks4 = "socks4";
    private const string Socks5 = "socks5";

    private readonly string _filename;
    private readonly CsvFileSettings _settings;
}