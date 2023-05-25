namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy verification state.
/// </summary>
public sealed class ProxyState
{
    private ProxyState() { }

    /// <summary>
    ///     Proxy verification state constructor.
    /// </summary>
    /// <param name="startTime">Check start time.</param>
    /// <param name="finishTime">Check finish time.</param>
    /// <param name="status">Proxy status.</param>
    public ProxyState(DateTime startTime, DateTime finishTime, ProxyStatus status)
    {
        StartTime = startTime;
        FinishTime = finishTime;
        Status = status;
    }

    /// <summary>
    ///     Creates an "undefined" proxy state.
    /// </summary>
    /// <remarks>This condition indicates that the proxy has not been checked yet.</remarks>
    /// <returns>Proxy state.</returns>
    public static ProxyState NotDefined => new();

    /// <summary>
    ///     Creates a proxy state that signals that the proxy check has been started, but has not finished yet.
    /// </summary>
    /// <param name="startTime">Check start time.</param>
    /// <returns>Proxy state.</returns>
    public static ProxyState StartChecking(DateTime startTime) => new() { StartTime = startTime };

    /// <summary>
    ///     Check start time.
    /// </summary>
    public DateTime? StartTime { get; private init; }

    /// <summary>
    ///     Check finish time.
    /// </summary>
    public DateTime? FinishTime { get; }

    /// <summary>
    ///     Proxy status. If Null, then the status is undefined.
    /// </summary>
    public ProxyStatus? Status { get; }
}