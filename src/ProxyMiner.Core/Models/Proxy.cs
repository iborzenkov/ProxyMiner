using System.Net;

namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy.
/// </summary>
public sealed class Proxy : IEquatable<Proxy>
{
    private Proxy(ProxyType type, string host, int port, ProxyAuthorizationData? authorizationData)
    {
        Host = host;
        Port = port;
        Type = type;
        AuthorizationData = authorizationData;

        Uri = new UriBuilder(GetScheme(), Host, Port).Uri;
    }

    /// <summary>
    ///     Proxy factory. Creates a valid proxy instance.
    /// </summary>
    public static IProxyFactory Factory { get; } = new ProxyFactory();

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
    ///     Proxy authorization data.
    /// </summary>
    /// <remarks> May be null.</remarks>
    public ProxyAuthorizationData? AuthorizationData { get; }

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
            && IsAuthorizationDataEquals(AuthorizationData, other.AuthorizationData);

        static bool IsAuthorizationDataEquals(ProxyAuthorizationData? ad1, ProxyAuthorizationData? ad2)
            => ad1 == null && ad2 == null || ad1 != null && ad1.Equals(ad2);
    }

    public override bool Equals(object? obj) 
        => ReferenceEquals(this, obj) || obj is Proxy other 
            && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Host, Port, Type, AuthorizationData);

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
    
    /// <summary>
    ///     Proxy factory.
    /// </summary>
    private sealed class ProxyFactory : IProxyFactory
    {
        public Proxy? TryMakeProxy(ProxyType type, string host, int port,
            out MakeProxyError? error, ProxyAuthorizationData? authorizationData = null)
        {
            return IsHostCorrected(host, out error) && IsPortCorrected(port, out error)
                ? new Proxy(type, host, port, authorizationData)        
                : null;
        }

        private static bool IsHostCorrected(string host, out MakeProxyError? proxyError)
        {
            proxyError = null;

            if (string.IsNullOrWhiteSpace(host))
            {
                proxyError = MakeProxyError.HostIsNull;
            }
            else if (host.Count(c => c == '.') != 3 || !IPAddress.TryParse(host, out var _))
            {
                proxyError = MakeProxyError.HostIsNotCorrect;
            }
            
            return proxyError == null;
        }

        private static bool IsPortCorrected(int port, out MakeProxyError? proxyError)
        {
            proxyError = null;
            if (port is < 0 or > 65535)
            {
                proxyError = MakeProxyError.PortOutOfRange;
            }
                
            return proxyError == null;
        }
    }
}