namespace ProxyMiner.Core.Models;

/// <summary>
///     Possible errors when creating a proxy by parameters.
/// </summary>
public enum MakeProxyError
{
    PortOutOfRange,
    HostIsNull,
    HostIsNotCorrect,
}