using System.Runtime.CompilerServices;

namespace ProxyMiner.Options;

public sealed class Settings
{
    public TimeSpan SourceTimeout
    {
        get => _sourceTimeout;
        set
        {
            if (_sourceTimeout == value)
                return;

            _sourceTimeout = value;
            OnChanged();
        }
    }

    public TimeSpan CheckTimeout
    {
        get => _checkTimeout;
        set
        {
            if (_checkTimeout == value)
                return;

            _checkTimeout = value;
            OnChanged();
        }
    }

    public TimeSpan SourceScanPeriod
    {
        get => _sourceScanPeriod;
        set
        {
            if (_sourceScanPeriod == value)
                return;

            _sourceScanPeriod = value;
            OnChanged();
        }
    }

    public int CheckThreadCount
    {
        get => _checkThreadCount;
        set
        {
            if (_checkThreadCount == value)
                return;

            _checkThreadCount = value;
            OnChanged();
        }
    }

    public TimeSpan ExpiredProxyActualState
    {
        get => _expiredProxyActualState;
        set
        {
            if (_expiredProxyActualState == value)
                return;

            _expiredProxyActualState = value;
            OnChanged();
        }
    }
    
    public event EventHandler<SettingsChangedEventArgs> Changed = (_, _) => { };

    private void OnChanged([CallerMemberName] string settingName = "")
    {
        Changed.Invoke(this, new SettingsChangedEventArgs(settingName));
    }

    private int _checkThreadCount = 10;
    private TimeSpan _sourceTimeout = TimeSpan.FromSeconds(10);
    private TimeSpan _checkTimeout = TimeSpan.FromSeconds(10);
    private TimeSpan _sourceScanPeriod = TimeSpan.FromMinutes(30);
    private TimeSpan _expiredProxyActualState = TimeSpan.FromSeconds(60);
}