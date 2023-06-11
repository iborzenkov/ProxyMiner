namespace ProxyMiner.Core.Options;

internal sealed class SettingsApplier
{
    internal SettingsApplier(Settings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    internal void Apply(Settings newSettings)
    {
        if (newSettings == null)
            throw new ArgumentNullException(nameof(newSettings));
        
        using var _ = _settings.BeginApply();
        
        _settings.SourceTimeout = newSettings.SourceTimeout;
        _settings.CheckTimeout = newSettings.CheckTimeout;
        _settings.SourceScanPeriod = newSettings.SourceScanPeriod;
        _settings.CheckThreadCount = newSettings.CheckThreadCount;
        _settings.ExpiredProxyActualState = newSettings.ExpiredProxyActualState;
    }

    private readonly Settings _settings;
}