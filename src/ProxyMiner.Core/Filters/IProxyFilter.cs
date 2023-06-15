using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Filters;

public interface IProxyFilter
{
    /// <summary>
    ///     Getting the proxies from the collection by the specified filter.
    /// </summary>
    /// <param name="filter">Filter.</param>
    /// <returns>Filtered proxy collection.</returns>
    IEnumerable<Proxy> Apply(Filter filter);
}