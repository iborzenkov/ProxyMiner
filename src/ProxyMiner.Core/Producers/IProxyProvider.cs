namespace ProxyMiner.Core.Producers;

/// <summary>
///     Provider of a proxies list.
/// </summary>
public interface IProxyProvider
{
    /// <summary>
    ///     Provides a list of proxies.
    /// </summary>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>Proxy list.</returns>
    Task<ProxyProviderResult> GetProxies(CancellationToken token);
}