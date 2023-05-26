namespace ProxyMiner.Core.Options;

public sealed class SettingsChangedEventArgs
{
    public SettingsChangedEventArgs(IEnumerable<string> settingNames)
    {
        SettingNames = settingNames;
    }

    public bool IsThisProperty(string settingName) => !SettingNames.Any()
        || SettingNames.Contains(settingName);

    public IEnumerable<string> SettingNames { get; }
}