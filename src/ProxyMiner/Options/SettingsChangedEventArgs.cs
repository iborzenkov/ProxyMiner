namespace ProxyMiner.Options;

public sealed class SettingsChangedEventArgs
{
    public SettingsChangedEventArgs(string settingName)
    {
        SettingName = settingName;
    }

    public bool IsThisProperty(string settingName) => string.IsNullOrEmpty(SettingName)
        || SettingName.Equals(settingName, StringComparison.Ordinal);

    public string SettingName { get; }
}