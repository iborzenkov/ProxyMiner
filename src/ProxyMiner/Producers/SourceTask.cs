using System.Timers;
using ProxyMiner.Options;
using Timer = System.Timers.Timer;

namespace ProxyMiner.Producers;

internal sealed class SourceTask : IDisposable
{
    public SourceTask(
        Producer producer, Settings settings, 
        Action<Producer, DateTime> miningStart,
        Action<Producer, DateTime, IEnumerable<Proxy>> miningFinished)
    {
        _producer = producer;
        _settings = settings;
        
        _miningStart = miningStart;
        _miningFinished = miningFinished;
    }

    public void Start()
    {
        if (_isActive)
            return;

        _isActive = true;

        _settings.Changed += SettingsChanged;
        _commonTokenSource = new CancellationTokenSource();

        _timerInited = false;
        _timer = new Timer
        {
            AutoReset = false,
            Enabled = true
        };
        _timer.Elapsed += TimerElapsed;
    }

    private async void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_timer != null && !_timerInited)
        {
            _timerInited = true;
            _timer.Interval = _settings.SourceScanPeriod.TotalMilliseconds;
        }

        if (_commonTokenSource == null || _commonTokenSource.Token.IsCancellationRequested)
            return;

        var startTime = DateTime.UtcNow;

        using var timeoutTokenSource = new CancellationTokenSource(_settings.SourceTimeout);
        using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            _commonTokenSource.Token,
            timeoutTokenSource.Token);

        var proxies = new List<Proxy>();
        _miningStart(_producer, startTime);
        try
        {
            proxies.AddRange(await _producer.Provider.GetProxies(linkedTokenSource.Token));
            
            _timer?.Start();
        }
        catch (TaskCanceledException) { }
        catch (OperationCanceledException) { }
        finally
        {
            _miningFinished(_producer, DateTime.UtcNow, proxies);
        }
    }

    public void Stop()
    {
        if (!_isActive)
            return;

        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;

        _commonTokenSource?.Cancel();
        _commonTokenSource?.Dispose();

        _settings.Changed -= SettingsChanged;

        _isActive = false;
    }

    public void Dispose() => Stop();

    private void SettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        if (e.IsThisProperty(nameof(Settings.SourceScanPeriod)) && _timer != null)
        {
            _timer.Interval = _settings.SourceScanPeriod.TotalMilliseconds;
        }
    }

    private readonly Producer _producer;
    private readonly Settings _settings;
    private readonly Action<Producer, DateTime> _miningStart;
    private readonly Action<Producer, DateTime, IEnumerable<Proxy>> _miningFinished;

    private Timer? _timer;
    private bool _timerInited;

    private CancellationTokenSource? _commonTokenSource;
    private bool _isActive;
}