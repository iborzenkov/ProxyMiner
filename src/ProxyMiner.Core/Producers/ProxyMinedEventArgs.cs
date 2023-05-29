using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Producers;

/// <summary>
///     Arguments of the event about the end of the proxy search in the source.
/// </summary>
public sealed class ProxyMinedEventArgs : EventArgs
{
    public ProxyMinedEventArgs(Producer producer, DateTime finishTimeUtc, IEnumerable<Proxy> proxies)
    {
        Producer = producer;
        FinishTimeUtc = finishTimeUtc;
        Proxies = proxies;
    }

    /// <summary>
    ///     The producer where the proxy search ended.
    /// </summary>
    public Producer Producer { get; }

    /// <summary>
    ///     The end time of the search.
    /// </summary>
    public DateTime FinishTimeUtc { get; }

    /// <summary>
    ///     The collection of found proxies.
    /// </summary>
    public IEnumerable<Proxy> Proxies { get; }
}