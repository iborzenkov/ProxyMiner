using System.Collections.Concurrent;
using ProxyMiner.Core.Filters;
using ProxyMiner.Core.Models.BaseCollections;

namespace ProxyMiner.Core.Models.ProxyCollections;

internal sealed class ProxyCollection : IProxyCollection
{
    public void Add(Proxy proxy) => AddRange(new[] { proxy });

    public void AddRange(IEnumerable<Proxy> proxies)
    {
        if (proxies == null)
            return;

        var addedItems = new List<Proxy>();
        foreach (var proxy in proxies.Where(p => p != null).ToList())
        {
            if (!_items.ContainsKey(proxy) && _items.TryAdd(proxy, ProxyState.NotDefined))
            {
                addedItems.Add(proxy);
            }
        }

        if (addedItems.Any())
        {
            OnCollectionChanged(CollectionChangedEventArgs<Proxy>.AddEventArgs(addedItems));
        }
    }

    public void Remove(Proxy proxy) => RemoveRange(new[] { proxy });

    public void RemoveRange(IEnumerable<Proxy> proxies)
    {
        if (proxies == null)
            return;

        var removedItems = proxies.Where(proxy => proxy != null && _items.TryRemove(proxy, out _)).ToList();

        if (removedItems.Any())
        {
            OnCollectionChanged(CollectionChangedEventArgs<Proxy>.RemoveEventArgs(removedItems));
        }
    }

    public IEnumerable<Proxy> Items => _items.Keys;

    public IEnumerable<Proxy> GetProxies(Filter filter)
    {
        var proxyWithState = _items.ToDictionary(i => i.Key, i => i.Value);
        var applier = new FilterApplier(proxyWithState);
        return applier.Apply(filter);
    }

    public event EventHandler<CollectionChangedEventArgs<Proxy>> CollectionChanged = (_, _) => { };

    internal void SetProxyState(Proxy proxy, ProxyState state)
    {
        // todo: here you can add an item to the collection that was previously deleted by another thread
        if (_items.ContainsKey(proxy))
        {
            _items[proxy] = state;
        }
    }

    private void OnCollectionChanged(CollectionChangedEventArgs<Proxy> args)
        => CollectionChanged.Invoke(this, args);

    private readonly ConcurrentDictionary<Proxy, ProxyState> _items = new();
}