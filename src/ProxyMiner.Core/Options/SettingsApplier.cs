namespace ProxyMiner.Core.Options;

internal sealed class SettingsApplier
{
    public SettingsApplier(Settings settings)
    {
        _settings = settings;
    }

    public void Apply(Settings newSettings)
    {
        using var _ = _settings.BeginApply();
        
        _settings.SourceTimeout = newSettings.SourceTimeout;
        _settings.CheckTimeout = newSettings.CheckTimeout;
        _settings.SourceScanPeriod = newSettings.SourceScanPeriod;
        _settings.CheckThreadCount = newSettings.CheckThreadCount;
        _settings.ExpiredProxyActualState = newSettings.ExpiredProxyActualState;
    }

    private readonly Settings _settings;
}