namespace ProxyMiner.Core.Options;

/// <summary>
///     Settings reader from file.
/// </summary>
public interface ISettingsFileReader
{
    /// <summary>
    ///     Read settings from file.
    /// </summary>
    /// <param name="fileName">File with settings.</param>
    /// <param name="settings">Read settings, if the reading is successful.</param>
    /// <returns>True if the file was read successfully.</returns>
    public bool TryRead(string fileName, out Settings? settings);
}