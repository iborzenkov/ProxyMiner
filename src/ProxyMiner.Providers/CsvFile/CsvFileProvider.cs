using ProxyMiner.Core.Models;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Providers.CsvFile;

/// <summary>
///     Retrieves the proxy list from a CSV-file.
/// </summary>
public sealed class CsvFileProvider : IProxyProvider
{
    public CsvFileProvider(string filename, CsvFileSettings settings)
    {
        _filename = filename;
        _settings = settings;
    }

    public Task<IEnumerable<Proxy>> GetProxies(CancellationToken token)
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