using ProxyMiner.Core.Filters;

namespace ProxyMiner.Core.Models.ProxyCollections;

/// <summary>
///     Proxy collection.
/// </summary>
public interface IProxyCollection
{
    /// <summary>
    ///     Adds one proxy to the collection.
    /// </summary>
    /// <param name="proxy">Proxy being added.</param>
    void Add(Proxy proxy);

    /// <summary>
    ///     Adds multiple proxies to the collection.
    /// </summary>
    /// <param name="proxies">Added proxies.</param>
    void AddRange(IEnumerable<Proxy> proxies);

    /// <summary>
    ///     Removes one proxy from the collection.
    /// </summary>
    /// <param name="proxy">Proxy being removed.</param>
    void Remove(Proxy proxy);

    /// <summary>
    ///     Removes multiple proxies from the collection.
    /// </summary>
    /// <param name="proxies">Removing proxies.</param>
    void RemoveRange(IEnumerable<Proxy> proxies);

    /// <summary>
    ///     Proxy in the collection.
    /// </summary>
    IEnumerable<Proxy> Items { get; }

    /// <summary>
    ///     Getting a proxy from the collection by the specified filter.
    /// </summary>
    /// <param name="filter">Filter.</param>
    /// <returns>Filtered proxy collection.</returns>
    IEnumerable<Proxy> GetProxies(Filter filter);

    /// <summary>
    ///     Event about changing items in the collection.
    /// </summary>
    event EventHandler<CollectionChangedEventArgs<Proxy>> CollectionChanged;
}