using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models.ProxyCollections;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Core;

public interface IMiner
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
    ///     The controlling mechanism for checking the proxy for validity.
    /// </summary>
    ICheckerController Checker { get; }
}
