namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy.
/// </summary>
public sealed class Proxy : IEquatable<Proxy>
{
    /// <summary>
    ///     Proxy constructor without authorization data.
    /// </summary>
    /// <param name="type">Proxy type.</param>
    /// <param name="host">Proxy host.</param>
    /// <param name="port">Proxy port.</param>
    public Proxy(ProxyType type, string host, int port) 
    {
        Host = host;
        Port = port;
        Type = type;

        Uri = new UriBuilder(GetScheme(), Host, Port).Uri;
    }

    /// <summary>
    ///     Proxy constructor with authorization data.
    /// </summary>
    /// <param name="type">Proxy type.</param>
    /// <param name="host">Proxy host.</param>
    /// <param name="port">Proxy port.</param>
    /// <param name="username">Proxy username.</param>
    /// <param name="password">Proxy password.</param>
    public Proxy(ProxyType type, string host, int port, string username, string password)
        : this(type, host, port)
    {
        Username= username;
        Password= password;
    }

    /// <summary>
    ///     Proxy host.
    /// </summary>
    public string Host { get; }
    
    /// <summary>
    ///     Proxy port.
    /// </summary>
    public int Port { get; }

    /// <summary>
    ///     Proxy type.
    /// </summary>
    public ProxyType Type { get; }

    /// <summary>
    ///     Proxy username.
    /// </summary>
    /// <remarks> May be null.</remarks>
    public string? Username { get; }

    /// <summary>
    ///     Proxy password.
    /// </summary>
    /// <remarks> May be null.</remarks>
    public string? Password { get; }

    /// <summary>
    ///     Proxy Uri.
    /// </summary>
    public Uri Uri { get; }

    public bool Equals(Proxy? other)
    {
        if (other is null) 
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
        => obj is not null && obj.GetType() == GetType() && Equals((Proxy)obj);

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