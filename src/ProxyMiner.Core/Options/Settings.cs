using System.Runtime.CompilerServices;

namespace ProxyMiner.Core.Options;

/// <summary>
///     ProxyMiner settings.
/// </summary>
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

    internal IDisposable BeginApply()
    {
        _groupingTransaction = new GroupingTransaction(this);
        return _groupingTransaction;
    }

    private void OnChanged([CallerMemberName] string settingName = "")
    {
        if (_groupingTransaction == null)
        {
            Changed.Invoke(this, new SettingsChangedEventArgs(new[] { settingName }));
        }
        else
        {
            _groupingTransaction.AddChangedSettingName(settingName);
        }
    }

    private int _checkThreadCount = 10;
    private TimeSpan _sourceTimeout = TimeSpan.FromSeconds(10);
    private TimeSpan _checkTimeout = TimeSpan.FromSeconds(10);
    private TimeSpan _sourceScanPeriod = TimeSpan.FromMinutes(30);
    private TimeSpan _expiredProxyActualState = TimeSpan.FromSeconds(60);
    
    private GroupingTransaction? _groupingTransaction;

    private sealed class GroupingTransaction : IDisposable
    {
        public GroupingTransaction(Settings settings) => _settings = settings;

        public void Dispose()
        {
            _settings._groupingTransaction = null;
            
            if (_isNeedChangeAllSettings || _applingSettingsName.Any())
            {
                _settings.Changed.Invoke(this, new SettingsChangedEventArgs(_applingSettingsName));
            }
        }

        public void AddChangedSettingName(string settingName)
        {
            if (_isNeedChangeAllSettings)
                return;

            if (string.IsNullOrEmpty(settingName))
            {
                _applingSettingsName.Clear();
                _isNeedChangeAllSettings = true;
            }
            else
            {
                if (!_applingSettingsName.Contains(settingName, StringComparer.OrdinalIgnoreCase))
                {
                    _applingSettingsName.Add(settingName);
                }
            }
        }

        private bool _isNeedChangeAllSettings;
        private readonly HashSet<string> _applingSettingsName = new();
        private readonly Settings _settings;

    }
}