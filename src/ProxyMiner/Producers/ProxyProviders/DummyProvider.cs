namespace ProxyMiner.Producers.ProxyProviders;

/// <summary>
///     Dummy proxy provider
/// </summary>
public sealed class DummyProvider : BaseProvider
{
    public override Task<IEnumerable<Proxy>> GetProxies(CancellationToken token)
    {
        return Task.Run(() =>
            {
                Thread.Sleep(5000);
                
                token.ThrowIfCancellationRequested();

                return Task.FromResult<IEnumerable<Proxy>>(new []
                {
                    new Proxy(ProxyType.Http, "1.1.1.1", 11),
                    new Proxy(ProxyType.Socks4, "2.2.2.2", 22),
                    new Proxy(ProxyType.Socks5, "3.3.3.3", 33)
                });
            },
            token);
    }
}