using System.Net;

namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy status factory. Creates various proxy statuses.
/// </summary>
public static class ProxyStatusFactory
{
    /// <summary>
    ///     Creates a proxy status with the status "Not anonymous".
    /// </summary>
    public static ProxyStatus ErrorStatus(HttpStatusCode status) => new() { Status = status };

    /// <summary>
    ///     Creates a proxy status with the status "Not anonymous".
    /// </summary>
    public static ProxyStatus NotAnonimous => new() { Status = HttpStatusCode.OK };

    /// <summary>
    ///     Creates a proxy status with the status "Anonymous".
    /// </summary>
    public static ProxyStatus Anonimous => new() { Status = HttpStatusCode.OK, IsAnonimous = true };

    /// <summary>
    ///     Creates a proxy status with the sign "verification canceled".
    /// </summary>
    public static ProxyStatus Cancelled => new() { IsCancelled = true };
}
