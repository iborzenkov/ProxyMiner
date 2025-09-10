using ProxyMiner.Core.Models;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Providers.CsvFile;

/// <summary>
///     Retrieves the proxy list from a CSV-file.
/// </summary>
public sealed class CsvFileProvider : IProxyProvider
{
    /// <summary>
    ///     Initializes a new instance of the CsvFileProvider class.
    /// </summary>
    /// <param name="filename">The path to the CSV file containing proxy data.</param>
    /// <param name="settings">The settings for parsing the CSV file.</param>
    public CsvFileProvider(string filename, CsvFileSettings settings)
    {
        _filename = filename;
        _settings = settings;
    }

    /// <summary>
    ///     Retrieves proxies from the specified CSV file.
    /// </summary>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>Proxy provider result containing proxies parsed from the CSV file.</returns>
    public Task<ProxyProviderResult> GetProxies(CancellationToken token)
    {
        return File.Exists(_filename)
            ? Task.Run(() =>
                {
                    var proxies = new List<Proxy>();
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
                            var authorizationData = username == null || password == null
                                ? null
                                : new ProxyAuthorizationData(username, password);
                            
                            var proxy  = Proxy.Factory.TryMakeProxy(type, host, port, out _, authorizationData);
                            if (proxy != null)
                            {
                                proxies.Add(proxy);
                            }
                        }
                        catch
                        {
                            // skip the whole line
                        }
                    }

                    return ProxyProviderResult.Ok(proxies);
                },
                token)
            : Task.FromResult(ProxyProviderResult.Error(new FileNotFoundException($"File '{_filename}' not found")));
    }

    private ProxyType GetProxyType(string proxyTypeAsString)
    {
        if (proxyTypeAsString.Equals(_settings.HttpTag, StringComparison.OrdinalIgnoreCase))
            return ProxyType.Http;

        if (proxyTypeAsString.Equals(_settings.Socks4Tag, StringComparison.OrdinalIgnoreCase))
            return ProxyType.Socks4;

        if (proxyTypeAsString.Equals(_settings.Socks5Tag, StringComparison.OrdinalIgnoreCase))
            return ProxyType.Socks5;

        throw new ArgumentOutOfRangeException($"Proxy type in CSV ({proxyTypeAsString}) is unknown");
    }

    private readonly string _filename;
    private readonly CsvFileSettings _settings;
}