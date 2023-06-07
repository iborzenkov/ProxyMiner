namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy verification state.
/// </summary>
public sealed record ProxyState
{
    private ProxyState() { }

    /// <summary>
    ///     Proxy verification state constructor.
    /// </summary>
    /// <param name="startTimeUtc">Check start time.</param>
    /// <param name="finishTimeUtc">Check finish time.</param>
    /// <param name="status">Proxy status.</param>
    internal ProxyState(DateTime startTimeUtc, DateTime finishTimeUtc, ProxyStatus status)
    {
        if (finishTimeUtc < startTimeUtc)
            throw new ArgumentOutOfRangeException(nameof(finishTimeUtc), "'finishTimeUtc' should be greater than the 'startTimeUtc'");
        
        StartTimeUtc = startTimeUtc;
        FinishTimeUtc = finishTimeUtc;
        Status = status;
    }

    /// <summary>
    ///     Creates an "undefined" proxy state.
    /// </summary>
    /// <remarks>This condition indicates that the proxy has not been checked yet.</remarks>
    /// <returns>Proxy state.</returns>
    internal static ProxyState NotDefined => new();

    /// <summary>
    ///     Creates a proxy state that signals that the proxy check has been started, but has not finished yet.
    /// </summary>
    /// <param name="startTimeUtc">Check start time.</param>
    /// <returns>Proxy state.</returns>
    internal static ProxyState StartChecking(DateTime startTimeUtc) => new() { StartTimeUtc = startTimeUtc };

    /// <summary>
    ///     Check start time.
    /// </summary>
    public DateTime? StartTimeUtc { get; private init; }

    /// <summary>
    ///     Check finish time.
    /// </summary>
    public DateTime? FinishTimeUtc { get; }

    /// <summary>
    ///     Proxy status. If Null, then the status is undefined.
    /// </summary>
    public ProxyStatus? Status { get; }
}