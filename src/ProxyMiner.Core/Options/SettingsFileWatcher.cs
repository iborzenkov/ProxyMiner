namespace ProxyMiner.Core.Options;

/// <summary>
///     Monitors the settings file, when it is changed, reads the settings from the file.
/// </summary>
internal sealed class SettingsFileWatcher : IDisposable
{
    public SettingsFileWatcher(ISettingsFileReader reader, string filename, Action<Settings> applySettings)
    {
        _reader = reader;
        _applySettings = applySettings;

        var folder = Path.GetDirectoryName(Path.GetFullPath(filename));
        if (!Directory.Exists(folder))
            return;

        _filename = Path.Combine(folder, filename);

        if (_reader.TryRead(_filename, out var settings))
        {
            _applySettings(settings!);
        }

        _watcher = new FileSystemWatcher(folder)
        {
            Filter = Path.GetFileName(Path.GetFileName(filename)),
            EnableRaisingEvents = true
        };
        _watcher.Changed += FolderChanged;
        _watcher.Created += FolderChanged;
    }

    void IDisposable.Dispose()
    {
        if (_watcher == null)
            return;

        _watcher.Changed -= FolderChanged;
        _watcher.Created -= FolderChanged;
        _watcher.Dispose();
    }

    private void FolderChanged(object sender, FileSystemEventArgs e)
    { 
        if (_reader.TryRead(_filename!, out var settings))
        {
            _applySettings(settings!);
        }
    }

    private readonly ISettingsFileReader _reader;
    private readonly Action<Settings> _applySettings;
    private readonly FileSystemWatcher? _watcher;
    private readonly string? _filename;
}