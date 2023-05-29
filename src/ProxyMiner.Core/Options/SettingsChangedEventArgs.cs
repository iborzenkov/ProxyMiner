﻿namespace ProxyMiner.Core.Options;

/// <summary>
///     Arguments of the event about the changing settings.
/// </summary>
public sealed class SettingsChangedEventArgs : EventArgs
{
    public SettingsChangedEventArgs(IEnumerable<string> settingNames)
    {
        SettingNames = settingNames;
    }

    /// <summary>
    ///     Is the specified setting present in the list of changed settings.
    /// </summary>
    /// <param name="settingName">Setting.</param>
    /// <returns>True if the specified setting is present in the list of changed settings.</returns>
    public bool IsThisProperty(string settingName) => !SettingNames.Any()
        || SettingNames.Contains(settingName);

    /// <summary>
    ///     List of changed settings.
    /// </summary>
    public IEnumerable<string> SettingNames { get; }
}