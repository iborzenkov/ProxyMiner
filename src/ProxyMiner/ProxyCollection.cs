using System.Collections.Concurrent;
using ProxyMiner.Filters;

namespace ProxyMiner;

internal sealed class ProxyCollection : IProxyCollection
{
    public void Add(Proxy proxy)
    {
        AddRange(new[] { proxy });
    }

    public void AddRange(IEnumerable<Proxy> proxies)
    {
        var addedItems = new List<Proxy>();
        foreach (var proxy in proxies.ToList())
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

    public void Remove(Proxy proxy)
    {
        RemoveRange(new[] { proxy });
    }

    public void RemoveRange(IEnumerable<Proxy> proxies)
    {
        var removedItems = proxies.Where(proxy => _items.TryRemove(proxy, out _)).ToList();

        if (removedItems.Any())
        {
            OnCollectionChanged(CollectionChangedEventArgs<Proxy>.RemoveEventArgs(removedItems));
        }
    }

    public IEnumerable<Proxy> Items => _items.Keys;

    public IEnumerable<Proxy> GetProxies(Filter filter)
    {
        var proxyWithState = _items.ToDictionary(i => i.Key, i => i.Value);
        var allProxies = proxyWithState.Select(p => p.Key).ToList();
        var withoutExcluded = allProxies.Except(filter.ExcludedProxies).ToList();
        var valided = GetValidProxies(withoutExcluded, filter.IsValid);
        var anonimous = GetAnonimousProxies(valided, filter.IsAnonimous);
        var withNotActualStates = GetProxiesWithNotActualStates(anonimous, filter.ExpiredState);
        var sorted = SortIfNeed(withNotActualStates, filter.Sort);
        sorted.InsertRange(0, filter.IncludedProxies.Except(filter.ExcludedProxies));
        return filter.Count == null
            ? sorted
            : sorted.Take(filter.Count ?? sorted.Count).ToList();

        List<Proxy> SortIfNeed(List<Proxy> proxies, ProxySort? sort)
        {
            if (sort != null)
            {
                proxies.Sort(ProxyComparerFactory.Make(sort, proxy => proxyWithState[proxy]));
            }

            return proxies;
        }

        List<Proxy> GetProxiesWithNotActualStates(List<Proxy> proxies, TimeSpan? expiredState)
        {
            if (expiredState == null)
                return proxies;

            var result = new List<Proxy>();

            foreach (var proxy in proxies)
            {
                var state = proxyWithState[proxy];
                if (state.FinishTime == null)
                {
                    result.Add(proxy);
                }
                else
                {
                    if (state.FinishTime.Value + expiredState.Value <= DateTime.UtcNow)
                        result.Add(proxy);
                }
            }

            return result;
        }
        
        List<Proxy> GetValidProxies(List<Proxy> proxies, bool? isValid)
        {
            if (isValid == null)
                return proxies;

            var result = new List<Proxy>();
            foreach (var proxy in proxies)
            {
                var state = proxyWithState[proxy];
                if (state.Status == null)
                    continue;

                if (state.Status.IsValid && isValid.Value || !state.Status.IsValid && !isValid.Value)
                    result.Add(proxy);
            }

            return result;
        }
        
        List<Proxy> GetAnonimousProxies(List<Proxy> proxies, bool? isAnonimous)
        {
            if (isAnonimous == null)
                return proxies;

            var result = new List<Proxy>();
            foreach (var proxy in proxies)
            {
                var state = proxyWithState[proxy];
                if (state.Status == null)
                    continue;

                if (state.Status.IsAnonimous && isAnonimous.Value || !state.Status.IsAnonimous && !isAnonimous.Value)
                    result.Add(proxy);
            }

            return result;
        }
    }

    public IEnumerable<Proxy> GetRandomProxies(Filter filter)
    {
        throw new NotImplementedException();
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