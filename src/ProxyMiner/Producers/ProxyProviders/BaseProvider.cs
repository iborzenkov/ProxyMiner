namespace ProxyMiner.Producers.ProxyProviders;

public abstract class BaseProvider : IProxyProvider
{
    public abstract Task<IEnumerable<Proxy>> GetProxies(CancellationToken token);
}