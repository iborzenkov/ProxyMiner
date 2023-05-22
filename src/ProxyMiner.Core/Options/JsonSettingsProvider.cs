using System.Text.Json;

namespace ProxyMiner.Core.Options;

public sealed class JsonSettingsProvider : ISettingsProvider, IDisposable
{
    public JsonSettingsProvider(string filename)
    {
        var settingsReader = new JsonReader(Settings);
        _settingsWatcher = new SettingsFileWatcher(settingsReader, filename);
    }
    
    public void Dispose()
    {
        _settingsWatcher.Dispose();
    }

    public Settings Settings { get; } = new();

    private readonly SettingsFileWatcher _settingsWatcher;
    
    private sealed class JsonReader : ISettingsFileReader
    {
        public JsonReader(Settings settings)
        {
            _settings = settings;
        }

        public void TryRead(string filename)
        {
            if (!File.Exists(filename))
                return;

            try
            {
                using var fileStream = new FileStream(filename, FileMode.Open);
                var settings = JsonSerializer.Deserialize<SettingsDto>(fileStream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true } );
                if (settings == null)
                    return;

                _settings.CheckThreadCount = settings.CheckThreadCount;

                _settings.SourceScanPeriod = settings.SourceScanPeriodSeconds == -1
                    ? TimeSpan.MaxValue
                    : TimeSpan.FromSeconds(settings.SourceScanPeriodSeconds);
                    
                _settings.SourceTimeout = settings.SourceTimeoutSeconds == -1
                    ? Timeout.InfiniteTimeSpan
                    : TimeSpan.FromSeconds(settings.SourceTimeoutSeconds);
                    
                _settings.CheckTimeout = settings.CheckTimeoutSeconds == -1
                    ? Timeout.InfiniteTimeSpan
                    : TimeSpan.FromSeconds(settings.CheckTimeoutSeconds);
                    
                _settings.ExpiredProxyActualState = settings.ExpiredProxyActualStateSeconds == -1
                    ? TimeSpan.MaxValue
                    : TimeSpan.FromSeconds(settings.ExpiredProxyActualStateSeconds);
            }
            catch
            {
                // ignored
            }
        }

        private readonly Settings _settings;
    }

    private class SettingsDto
    {
        public int SourceTimeoutSeconds { get; set; }
        public int CheckTimeoutSeconds { get; set; }
        public int SourceScanPeriodSeconds { get; set; }
        public int CheckThreadCount { get; set; }
        public int ExpiredProxyActualStateSeconds { get; set; }
    }
}