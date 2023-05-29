using System.Text.Json;

namespace ProxyMiner.Core.Options;

/// <summary>
///     Settings provider from JSON-file.
/// </summary>
public sealed class JsonSettingsProvider : ISettingsProvider, IDisposable
{
    public JsonSettingsProvider(string filename)
    {
        var settingsReader = new JsonReader();
        var settingsApplier = new SettingsApplier(Settings);
        _settingsWatcher = new SettingsFileWatcher(settingsReader, filename, settings => settingsApplier.Apply(settings));
    }
    
    public void Dispose()
    {
        _settingsWatcher.Dispose();
    }

    public Settings Settings { get; } = new();

    private readonly SettingsFileWatcher _settingsWatcher;
    
    private sealed class JsonReader : ISettingsFileReader
    {
        public bool TryRead(string filename, out Settings? settings)
        {
            settings = null;
            if (!File.Exists(filename))
                return false;

            try
            {
                using var fileStream = new FileStream(filename, FileMode.Open);
                var settingsDto = JsonSerializer.Deserialize<SettingsDto>(fileStream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (settingsDto == null)
                    return false;

                settings = new Settings
                {
                    CheckThreadCount = settingsDto.CheckThreadCount,
                    SourceScanPeriod = settingsDto.SourceScanPeriodSeconds == -1
                        ? TimeSpan.MaxValue
                        : TimeSpan.FromSeconds(settingsDto.SourceScanPeriodSeconds),
                    SourceTimeout = settingsDto.SourceTimeoutSeconds == -1
                        ? Timeout.InfiniteTimeSpan
                        : TimeSpan.FromSeconds(settingsDto.SourceTimeoutSeconds),
                    CheckTimeout = settingsDto.CheckTimeoutSeconds == -1
                        ? Timeout.InfiniteTimeSpan
                        : TimeSpan.FromSeconds(settingsDto.CheckTimeoutSeconds),
                    ExpiredProxyActualState = settingsDto.ExpiredProxyActualStateSeconds == -1
                        ? TimeSpan.MaxValue
                        : TimeSpan.FromSeconds(settingsDto.ExpiredProxyActualStateSeconds)
                };
                return true;
            }
            catch
            {
                return false;
            }
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
}