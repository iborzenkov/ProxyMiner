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
    /// <param name="fileName">Read settings.</param>
    /// <returns>True if the file was read successfully.</returns>
    public bool TryRead(string fileName, out Settings? settings);
}
