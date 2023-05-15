namespace ProxyMiner.Producers.ProxyProviders;

public interface IProxyProvider
{
    /// <summary>
    ///     Provides a list of proxies.
    /// </summary>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>Proxy list.</returns>
    Task<IEnumerable<Proxy>> GetProxies(CancellationToken token);
}