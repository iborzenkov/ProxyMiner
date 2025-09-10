using System.Net;

namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy status.
/// </summary>
public sealed record ProxyStatus
{
    private ProxyStatus() { }

    /// <summary>
    ///     Creates a proxy status with the status "Not anonymous".
    /// </summary>
    internal static ProxyStatus ErrorStatus(HttpStatusCode status) => new() { Status = status };

    /// <summary>
    ///     Creates a proxy status with the status "Not anonymous".
    /// </summary>
    internal static ProxyStatus NotAnonimous => new() { Status = HttpStatusCode.OK };

    /// <summary>
    ///     Creates a proxy status with the status "Anonymous".
    /// </summary>
    internal static ProxyStatus Anonimous => new() { Status = HttpStatusCode.OK, IsAnonimous = true };

    /// <summary>
    ///     Creates a proxy status with the sign "verification canceled by user".
    /// </summary>
    internal static ProxyStatus Cancelled => new() { IsCancelled = true };

    /// <summary>
    ///     Creates a proxy status with the sign "verification canceled by timeout".
    /// </summary>
    internal static ProxyStatus Timeout => new() { IsTimeout = true };

    /// <summary>
    ///     Status code, when connecting via proxy.
    /// </summary>
    public HttpStatusCode? Status { get; private init; }

    /// <summary>
    ///     Indicates that the proxy is anonymous.
    /// </summary>
    public bool IsAnonimous { get; private init; }

    /// <summary>
    ///     Indicates that the proxy check has been canceled by user.
    /// </summary>
    public bool IsCancelled { get; private init; }

    /// <summary>
    ///     Indicates that the proxy check has been canceled by timeout.
    /// </summary>
    public bool IsTimeout { get; private init; }    

    /// <summary>
    ///     A sign that the proxy is alive.
    /// </summary>
    /// <remarks>It does not say that the proxy is anonymous or not anonymous.</remarks>
    public bool IsValid => !IsCancelled && !IsTimeout && Status == HttpStatusCode.OK;

    public override string ToString()
    {
        if (IsCancelled)
            return "Cancelled";

        if (IsTimeout)
            return "Timeout";

        if (Status != HttpStatusCode.OK)
            return Status?.ToString() ?? "NULL";

        return IsAnonimous
            ? "Anonimous"
            : "Not Anonimous";
    }
}