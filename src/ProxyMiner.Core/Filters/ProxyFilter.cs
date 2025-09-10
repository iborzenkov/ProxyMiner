using System.Collections.Concurrent;
using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Models.BaseCollections;
using ProxyMiner.Core.Models.ProxyCollections;

namespace ProxyMiner.Core.Filters;

internal sealed class ProxyFilter : IProxyFilter, ICheckObserver
{
    internal ProxyFilter(ProxyCollection proxies, ProxyChecker checker)
    {
        _proxies = proxies ?? throw new ArgumentNullException(nameof(proxies));
        _proxies.CollectionChanged += Proxies_CollectionChanged;

        _checker = checker ?? throw new ArgumentNullException(nameof(checker));
        _checker.Subscribe(this);
    }

    IEnumerable<Proxy> IProxyFilter.Apply(Action<FilterBuilder> modifier)
    {
        var builder = new FilterBuilder();
        modifier.Invoke(builder);
        var filter = builder.Build();
        
        var applier = new FilterApplier(_proxiesStates.Values);
        return applier.Apply(filter);
    }

    void ICheckObserver.EnabledChanged(bool enabled)
    {
    }

    void ICheckObserver.Checking(ProxyCheckingEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        SetProxyState(new StateOfProxy(
            args.Proxy, ProxyState.StartChecking(args.StartTimeUtc)));
    }

    void ICheckObserver.Checked(ProxyCheckedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        SetProxyState(args.StateOfProxy);
    }

    private void SetProxyState(StateOfProxy proxyState)
    {
        ArgumentNullException.ThrowIfNull(proxyState);

        // todo: here you can add an item to the collection that was previously deleted by another thread
        if (_proxiesStates.ContainsKey(proxyState.Proxy))
        {
            _proxiesStates[proxyState.Proxy] = proxyState;
        }
    }

    private void Proxies_CollectionChanged(object? sender, CollectionChangedEventArgs<Proxy> e)
    {
        if (e.Action == CollectionChangeAction.Add)
        {
            foreach (var proxy in e.NewItems!)
            {
                _proxiesStates[proxy] = new StateOfProxy(proxy, ProxyState.NotDefined);
            }
        }

        if (e.Action == CollectionChangeAction.Remove)
        {
            foreach (var proxy in e.OldItems!)
            {
                _proxiesStates.TryRemove(proxy, out _);
            }
        }
    }

    private readonly ProxyCollection _proxies;
    private readonly ProxyChecker _checker;
    private readonly ConcurrentDictionary<Proxy, StateOfProxy> _proxiesStates = new();
}