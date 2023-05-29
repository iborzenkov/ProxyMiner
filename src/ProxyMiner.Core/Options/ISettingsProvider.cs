namespace ProxyMiner.Core.Options;

/// <summary>
///     Settings provider.
/// </summary>
public interface ISettingsProvider
{
    /// <summary>
    ///     Settings.
    /// </summary>
    Settings Settings { get; }
}