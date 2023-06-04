namespace ProxyMiner.Core.Models;

public interface IProxyFactory
{        
    /// <summary>
    ///     Make proxy with authorization data.
    /// </summary>
    /// <param name="type">Proxy type.</param>
    /// <param name="host">Proxy host.</param>
    /// <param name="port">Proxy port.</param>
    /// <param name="authorizationData">Authorization data.</param>
    (Proxy?, MakeProxyError?) TryMakeProxy(ProxyType type, string host, int port,
        ProxyAuthorizationData? authorizationData = null);
}