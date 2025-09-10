namespace ProxyMiner.Core.Models;

/// <summary>
///     Factory interface for creating proxy instances.
/// </summary>
public interface IProxyFactory
{
    /// <summary>
    ///     Make proxy with authorization data.
    /// </summary>
    /// <param name="type">Proxy type.</param>
    /// <param name="host">Proxy host.</param>
    /// <param name="port">Proxy port.</param>
    /// <param name="error">An error occurred when creating a proxy.</param>
    /// <param name="authorizationData">Authorization data.</param>
    Proxy? TryMakeProxy(ProxyType type, string host, int port,
        out MakeProxyError? error, ProxyAuthorizationData? authorizationData = null);
}