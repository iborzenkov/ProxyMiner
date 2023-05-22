using System.Collections.Concurrent;
using System.Timers;
using ProxyMiner.Core.Filters;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Models.ProxyCollections;
using ProxyMiner.Core.Options;
using Timer = System.Timers.Timer;

namespace ProxyMiner.Core.Checkers;

internal sealed class ProxyCheckerController : ICheckerController, ICheckObserver, IDisposable
{
    public ProxyCheckerController(ProxyCollection proxies, ProxyChecker checker, Settings settings)
    {
        _proxies = proxies;
        _settings = settings;

        _checker = checker;
        _checker.Subscribe(this);

        _timer = new Timer(TimerIntervalMilliseconds) { Enabled = _checker.IsEnabled };
        _timer.Elapsed += TimerElapsed;
    }

    public void Dispose()
    {
        _checker.Unsubscribe(this);

        _timer.Stop();
        _timer.Dispose();
    }

    public void CheckNow(IEnumerable<Proxy> proxies)
    {
        lock (_locker)
        {
            var collection = proxies.ToList();

            _highPriority.AddRange(collection.Except(_highPriority));
            _blocked.RemoveAll(proxy => collection.Contains(proxy));
        }
    }

    public void StopChecking(IEnumerable<Proxy> proxies)
    {
        lock (_locker)
        {
            var collection = proxies.ToList();

            _blocked.AddRange(collection.Except(_blocked));
            _highPriority.RemoveAll(proxy => collection.Contains(proxy));
        }
    }

    public event EventHandler<ProxyCheckingEventArgs> Checking = (_, _) => { };
    public event EventHandler<ProxyCheckedEventArgs> Checked = (_, _) => { };

    void ICheckObserver.EnabledChanged(bool enabled)
    {
        _inProgress.Clear();
        _timer.Enabled = enabled;
    }

    void ICheckObserver.Checking(ProxyCheckingEventArgs args)
    {
        _proxies.SetProxyState(args.Proxy, ProxyState.StartChecking(args.StartTime));
        Checking.Invoke(this, args);
    }

    void ICheckObserver.Checked(ProxyCheckedEventArgs args)
    {
        _proxies.SetProxyState(args.Proxy, args.State);

        _inProgress.TryRemove(args.Proxy, out _);
        Checked.Invoke(this, args);
    }
    
    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var proxies = _proxies.GetProxies(
            Filter.Builder
                .Count(_checker.FreeCheckSlot)
                .Except(_inProgress.Keys.Concat(_blocked))
                .Include(_highPriority)
                .StateNotExpired(_settings.ExpiredProxyActualState)
                .SortedBy(SortingField.LastCheck, SortDirection.Asceding)
                .Build()
            ).ToList();

        if (_highPriority.Any())
        {
            proxies.ForEach(proxy => _highPriority.Remove(proxy));
        }

        if (proxies.Count == 0)
            return;

        foreach (var proxy in proxies)
        {
            _inProgress.TryAdd(proxy, false);
        }
        
        _checker.Add(proxies);
    }

    private readonly ProxyCollection _proxies;
    private readonly Settings _settings;
    private readonly ProxyChecker _checker;
    private readonly Timer _timer;
    private readonly ConcurrentDictionary<Proxy,bool> _inProgress = new();
    private readonly List<Proxy> _highPriority = new();
    private readonly List<Proxy> _blocked = new();
    private readonly object _locker = new();

    private const int TimerIntervalMilliseconds = 3000;
}