namespace ProxyMiner.Core.Models;

public sealed class Proxy : IEquatable<Proxy>
{
    public Proxy(ProxyType type, string host, int port) 
        : this(type, host, port, username: null, password: null)
    {
    }

    public Proxy(ProxyType type, string host, int port, string? username, string? password)
    {
        Host = host;
        Port = port;
        Type = type;
        Username= username;
        Password= password;
        
        Uri = new UriBuilder(GetScheme(), Host, Port).Uri;
    }

    public string Host { get; }
    public int Port { get; }
    public ProxyType Type { get; }
    public string? Username { get; }
    public string? Password { get; }

    public Uri Uri { get; }

    public bool Equals(Proxy? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;

        return Host.Equals(other.Host, StringComparison.OrdinalIgnoreCase)
            && Port == other.Port
            && Type == other.Type
            && IsNullableStringsEquals(Username, other.Username)
            && IsNullableStringsEquals(Password, other.Password);

        static bool IsNullableStringsEquals(string? str1, string? str2)
        {
            return str1 == null && str2 == null
                || str1 != null && str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) 
            return false;
        if (ReferenceEquals(this, obj)) 
            return true;

        return obj.GetType() == GetType() && Equals((Proxy)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Host, Port, Type, Username, Password);

    private string GetScheme()
    {
        return Type switch
        {
            ProxyType.Http => "http",
            ProxyType.Socks4 => "socks4",
            ProxyType.Socks5 => "socks5",
            _ => throw new ArgumentOutOfRangeException($"The type {Type} is not implemented for scheme")
        };
    }
}