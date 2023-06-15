using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Filters;
using ProxyMiner.Core.Models.ProxyCollections;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Core;

public interface IMiner : IDisposable
{
    /// <summary>
    ///     Start mining.
    /// </summary>
    void Start();

    /// <summary>
    ///     Stop mining.
    /// </summary>
    void Stop();

    /// <summary>
    ///     Collection of producers.
    /// </summary>
    IProducerCollection Producers { get; }

    /// <summary>
    ///     Collection of proxy.
    /// </summary>
    IProxyCollection Proxies { get; }

    /// <summary>
    ///     Allows you to get a collection of proxies by the specified filter.
    /// </summary>
    IProxyFilter ProxyFilter { get; }

    /// <summary>
    ///     The controlling mechanism for checking the proxy for validity.
    /// </summary>
    ICheckerController Checker { get; }
}