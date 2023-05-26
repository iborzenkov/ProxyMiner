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

    /// <summary>
    ///     Status code, when connecting via proxy.
    /// </summary>
    public HttpStatusCode Status { get; init; }

    /// <summary>
    ///     Indicates that the proxy is anonymous.
    /// </summary>
    public bool IsAnonimous { get; init; }

    /// <summary>
    ///     Indicates that the proxy check has been canceled.
    /// </summary>
    public bool IsCancelled { get; init; }

    /// <summary>
    ///     A sign that the proxy is alive.
    /// </summary>
    /// <remarks>It does not say that the proxy is anonymous or not anonymous.</remarks>
    public bool IsValid => !IsCancelled && Status == HttpStatusCode.OK;

    public override string ToString()
    {
        if (IsCancelled)
            return "Cancelled";

        if (Status != HttpStatusCode.OK)
            return Status.ToString();

        return IsAnonimous
            ? "Anonimous"
            : "Not Anonimous";
    }
}