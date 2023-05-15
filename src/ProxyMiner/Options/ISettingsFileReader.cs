namespace ProxyMiner.Options;

public interface ISettingsFileReader
{
    public void TryRead(string fileName);
}
