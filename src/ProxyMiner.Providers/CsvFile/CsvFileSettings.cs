namespace ProxyMiner.Providers.CsvFile;

public sealed class CsvFileSettings
{
    public CsvFileSettings(int typeColumn, int hostColumn, int portColumn)
    {
        TypeColumn = typeColumn;
        HostColumn = hostColumn;
        PortColumn = portColumn;
    }

    public static CsvFileSettings Default => new (typeColumn: 0, hostColumn: 1, portColumn: 2);
    
    public int TypeColumn { get; }
    public int HostColumn { get; }
    public int PortColumn { get; }
    public int? UsernameColumn { get; init; }
    public int? PasswordColumn { get; init; }
    
    
    public string HttpTag { get; init; } = "http";
    public string Socks4Tag { get; init; } = "socks4";
    public string Socks5Tag { get; init; } = "socks5";
}