using ProxyMiner.Core.Models;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Providers;

/// <summary>
///     Dummy proxy provider.
/// </summary>
public sealed class DummyProvider : IProxyProvider
{
    public DummyProvider() : this(DefaulProxies) { }
    
    public DummyProvider(IEnumerable<Proxy> proxies) => _proxies = proxies;
    
    public Task<ProxyProviderResult> GetProxies(CancellationToken token)
    {
        return Task.Run(() =>
            {
                Thread.Sleep(5000);
                
                token.ThrowIfCancellationRequested();

                return Task.FromResult(ProxyProviderResult.Ok(_proxies));
            },
            token);
    }
    
    private readonly IEnumerable<Proxy> _proxies;
    
    private static readonly IEnumerable<Proxy> DefaulProxies = new []
    {
        new Proxy(ProxyType.Http, "1.1.1.1", 11),
        new Proxy(ProxyType.Socks4, "2.2.2.2", 22),
        new Proxy(ProxyType.Socks5, "3.3.3.3", 33)
    };
}