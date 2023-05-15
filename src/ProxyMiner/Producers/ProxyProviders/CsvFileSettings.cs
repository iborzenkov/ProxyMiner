namespace ProxyMiner.Producers.ProxyProviders;

public sealed class CsvFileSettings
{
    public CsvFileSettings(int typeColumn, int hostColumn, int portColumn, 
        int? usernameColumn, int? passwordColumn)
    {
        TypeColumn = typeColumn;
        HostColumn = hostColumn;
        PortColumn = portColumn;
        UsernameColumn = usernameColumn;
        PasswordColumn = passwordColumn;
    }

    public static CsvFileSettings Default => new (
        typeColumn: 0, hostColumn: 1, portColumn: 2, usernameColumn: null, passwordColumn: null);
    public int TypeColumn { get; }
    public int HostColumn { get; }
    public int PortColumn { get; }
    public int? UsernameColumn { get; }
    public int? PasswordColumn { get; }
}