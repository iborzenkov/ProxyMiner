using ProxyMiner.Core.Filters;
using ProxyMiner.Core.Models.BaseCollections;

namespace ProxyMiner.Core.Models.ProxyCollections;

/// <summary>
///     Proxy collection.
/// </summary>
public interface IProxyCollection : IBaseCollection<Proxy>
{
    /// <summary>
    ///     Getting a proxy from the collection by the specified filter.
    /// </summary>
    /// <param name="filter">Filter.</param>
    /// <returns>Filtered proxy collection.</returns>
    IEnumerable<Proxy> GetProxies(Filter filter);
}