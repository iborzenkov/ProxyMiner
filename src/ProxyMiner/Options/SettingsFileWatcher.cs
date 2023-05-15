namespace ProxyMiner.Options;

public sealed class SettingsFileWatcher : IDisposable
{
    public SettingsFileWatcher(ISettingsFileReader reader, string filename)
    {
        _reader = reader;

        var folder = Path.GetDirectoryName(Path.GetFullPath(filename));
        if (!Directory.Exists(folder))
            return;

        _filename = Path.Combine(folder, filename);
        
        _reader.TryRead(_filename);

        _watcher = new FileSystemWatcher(folder)
        {
            Filter = Path.GetFileName(Path.GetFileName(filename)),
            EnableRaisingEvents = true
        };
        _watcher.Changed += FolderChanged;
        _watcher.Created += FolderChanged;
    }

    public void Dispose()
    {
        if (_watcher == null)
            return;

        _watcher.Changed -= FolderChanged;
        _watcher.Created -= FolderChanged;
        _watcher.Dispose();
    }

    private void FolderChanged(object sender, FileSystemEventArgs e) 
        => _reader.TryRead(_filename!);

    private readonly ISettingsFileReader _reader;
    private readonly FileSystemWatcher? _watcher;
    private readonly string? _filename;
}