namespace ProxyMiner.Core.Options;

public interface ISettingsFileReader
{
    public void TryRead(string fileName);
}
