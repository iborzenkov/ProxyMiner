namespace ProxyMiner.Core.Options;

/// <summary>
///     Settings provider.
/// </summary>
public interface ISettingsProvider : IDisposable
{
    /// <summary>
    ///     Settings.
    /// </summary>
    Settings Settings { get; }
}