namespace ProxyMiner;

public sealed class ProxyState
{
    private ProxyState() { }

    public ProxyState(DateTime startTime, DateTime finishTime, ProxyStatus status)
    {
        StartTime = startTime;
        FinishTime = finishTime;
        Status = status;
    }

    public static ProxyState NotDefined => new();
    public static ProxyState StartChecking(DateTime startTime) => new() { StartTime = startTime };

    public DateTime? StartTime { get; private init; }
    public DateTime? FinishTime { get; }
    public ProxyStatus? Status { get; }
}