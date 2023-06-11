using System.Timers;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Options;
using Timer = System.Timers.Timer;

namespace ProxyMiner.Core.Producers;

internal sealed class ProducerTask : IDisposable
{
    public ProducerTask(
        Producer producer, Settings settings, 
        Action<Producer, DateTime> miningStart,
        Action<Producer, DateTime, ProxyProviderResult> miningFinished)
    {
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        
        _miningStart = miningStart ?? throw new ArgumentNullException(nameof(miningStart));
        _miningFinished = miningFinished ?? throw new ArgumentNullException(nameof(miningFinished));
    }

    public void Dispose() => Stop();

    internal void Start()
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

    internal void Stop()
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

    private async void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_timer != null && !_timerInited)
        {
            _timerInited = true;
            _timer.Interval = _settings.SourceScanPeriod.TotalMilliseconds;
        }

        if (_commonTokenSource == null || _commonTokenSource.Token.IsCancellationRequested)
            return;

        var startTimeUtc = DateTime.UtcNow;

        using var timeoutTokenSource = new CancellationTokenSource(_settings.SourceTimeout);
        using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            _commonTokenSource.Token,
            timeoutTokenSource.Token);

        var proxies = new List<Proxy>();
        _miningStart(_producer, startTimeUtc);

        ProxyProviderResult result = ProxyProviderResult.Unknown;
        try
        {
            result = await _producer.Provider.GetProxies(linkedTokenSource.Token);
            if (result.Code == ProxyProviderResultCode.Ok)
            {
                proxies.AddRange(result.Proxies);
            }

            _timer?.Start();
        }
        catch (OperationCanceledException) 
        {
            result = timeoutTokenSource.IsCancellationRequested
                ? ProxyProviderResult.Timeout
                : ProxyProviderResult.Cancelled;
        }
        catch (Exception exception)
        {
            result = ProxyProviderResult.Error(exception);
        }
        finally
        {
            _miningFinished(_producer, DateTime.UtcNow, result);
        }
    }

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
    private readonly Action<Producer, DateTime, ProxyProviderResult> _miningFinished;

    private Timer? _timer;
    private bool _timerInited;

    private CancellationTokenSource? _commonTokenSource;
    private bool _isActive;
}