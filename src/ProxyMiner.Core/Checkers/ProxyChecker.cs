using System.Collections.Concurrent;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Options;

namespace ProxyMiner.Core.Checkers;

internal sealed class ProxyChecker : IDisposable
{
    internal ProxyChecker(IChecker checker, Settings settings)
    {
        _checker = checker ?? throw new ArgumentNullException(nameof(checker));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public void Dispose() => Stop();

    internal bool IsEnabled
    {
        get => _isEnabled;
        private set
        {
            if (_isEnabled == value)
                return;

            _isEnabled = value;
            _observers.ForEach(o => o.EnabledChanged(_isEnabled));
        }
    }

    internal int FreeCheckSlot => 
        _proxies == null 
            ? 0 
            : _proxies.BoundedCapacity - _proxies.Count;

    internal void Start()
    {
        if (IsEnabled)
            return;

        IsEnabled = true;

        _commonTokenSource = new CancellationTokenSource();
        _proxies = new BlockingCollection<Proxy>(_settings.CheckThreadCount);

        for (var i = 0; i < _settings.CheckThreadCount; i++)
        {
            Task.Run(async () =>
            {
                try
                {
                    foreach (var proxy in _proxies.GetConsumingEnumerable(_commonTokenSource.Token))
                    {
                        using var timeoutTokenSource = new CancellationTokenSource(_settings.CheckTimeout);
                        using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                            _commonTokenSource.Token,
                            timeoutTokenSource.Token);

                        var startDate = DateTime.UtcNow;
                        _observers.ForEach(o => o.Checking(new ProxyCheckingEventArgs(proxy, startDate)));

                        try
                        {
                            var status = await _checker.Check(proxy, linkedTokenSource.Token);
                            _observers.ForEach(o => o.Checked(new ProxyCheckedEventArgs(
                                new StateOfProxy(proxy, new ProxyState(startDate, DateTime.UtcNow, status)))));
                        }
                        catch (OperationCanceledException)
                        {
                            var status = timeoutTokenSource.IsCancellationRequested
                                ? ProxyStatus.Timeout
                                : ProxyStatus.Cancelled;
                            _observers.ForEach(o => o.Checked(new ProxyCheckedEventArgs(
                                new StateOfProxy(proxy, new ProxyState(startDate, DateTime.UtcNow, status)))));
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // ignored
                }
            });
        }
    }

    internal void Stop()
    {
        if (!IsEnabled)
            return;

        IsEnabled = false;
        
        _commonTokenSource?.Cancel();
        _commonTokenSource?.Dispose();

        _proxies?.CompleteAdding();
        _proxies?.Dispose();
        _proxies = null;
    }

    internal void Add(IEnumerable<Proxy> proxies)
    {
        ArgumentNullException.ThrowIfNull(proxies);

        if (!IsEnabled)
            return;

        var localProxies = proxies.ToList();
        Task.Run(() =>
        {
            foreach (var proxy in localProxies)
            {
                if (_commonTokenSource!.IsCancellationRequested)
                    break;

                _proxies!.TryAdd(proxy, Timeout.Infinite, _commonTokenSource!.Token);
            }
        });
    }

    internal void Subscribe(ICheckObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _observers.Add(observer);
    }

    internal void Unsubscribe(ICheckObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _observers.Remove(observer);
    }

    private BlockingCollection<Proxy>? _proxies;
    private CancellationTokenSource? _commonTokenSource;
    private bool _isEnabled;

    private readonly Settings _settings;
    private readonly IChecker _checker;
    
    private readonly List<ICheckObserver> _observers = [];
}