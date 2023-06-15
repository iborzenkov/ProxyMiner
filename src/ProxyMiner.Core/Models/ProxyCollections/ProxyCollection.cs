using System.Collections.Concurrent;
using ProxyMiner.Core.Filters;
using ProxyMiner.Core.Models.BaseCollections;

namespace ProxyMiner.Core.Models.ProxyCollections;

internal sealed class ProxyCollection : IProxyCollection
{
    public void Add(Proxy proxy)
    {
        if (proxy == null)
            throw new ArgumentNullException(nameof(proxy));
        
        AddRange(new[] { proxy });
    }

    public void AddRange(IEnumerable<Proxy> proxies)
    {
        if (proxies == null)
            throw new ArgumentNullException(nameof(proxies));

        var addedItems = new List<Proxy>();
        foreach (var proxy in proxies.Where(proxy => proxy != null).ToList())
        {
            if (!_items.ContainsKey(proxy) && _items.TryAdd(proxy, IgnoredValue))
            {
                addedItems.Add(proxy);
            }
        }

        if (addedItems.Any())
        {
            OnCollectionChanged(CollectionChangedEventArgs<Proxy>.AddEventArgs(addedItems));
        }
    }

    public void Remove(Proxy proxy)
    {
        if (proxy == null)
            throw new ArgumentNullException(nameof(proxy));

        RemoveRange(new[] { proxy });
    }

    public void RemoveRange(IEnumerable<Proxy> proxies)
    {
        if (proxies == null)
            throw new ArgumentNullException(nameof(proxies));

        var removedItems = proxies.Where(proxy => 
            proxy != null && _items.TryRemove(proxy, out _)).ToList();

        if (removedItems.Any())
        {
            OnCollectionChanged(CollectionChangedEventArgs<Proxy>.RemoveEventArgs(removedItems));
        }
    }

    public IEnumerable<Proxy> Items => _items.Keys;

    public event EventHandler<CollectionChangedEventArgs<Proxy>> CollectionChanged = (_, _) => { };

    private void OnCollectionChanged(CollectionChangedEventArgs<Proxy> args)
        => CollectionChanged.Invoke(this, args);

    private readonly ConcurrentDictionary<Proxy, bool> _items = new();
    private const bool IgnoredValue = false;
}