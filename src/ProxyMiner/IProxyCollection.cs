using ProxyMiner.Filters;

namespace ProxyMiner;

public interface IProxyCollection
{
    void Add(Proxy proxy);

    void AddRange(IEnumerable<Proxy> proxies);

    void Remove(Proxy proxy);

    void RemoveRange(IEnumerable<Proxy> proxies);

    IEnumerable<Proxy> Items { get; }

    IEnumerable<Proxy> GetProxies(Filter filter);

    event EventHandler<CollectionChangedEventArgs<Proxy>> CollectionChanged;
}