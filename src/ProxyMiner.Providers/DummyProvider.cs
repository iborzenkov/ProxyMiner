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
        MakeProxy(ProxyType.Http, "1.1.1.1", 11),
        MakeProxy(ProxyType.Socks4, "2.2.2.2", 22),
        MakeProxy(ProxyType.Socks5, "3.3.3.3", 33)
    };

    private static Proxy MakeProxy(ProxyType type, string host, int port)
        => Proxy.Factory.TryMakeProxy(type, host, port, out _)!;
}