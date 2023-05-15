using System.Net;

namespace ProxyMiner;

public sealed class ProxyStatus
{
    private ProxyStatus() { }

    public static ProxyStatus ErrorStatus(HttpStatusCode status) => new() { Status = status };
    public static ProxyStatus NotAnonimous => new() { Status = HttpStatusCode.OK };
    public static ProxyStatus Anonimous => new() { Status = HttpStatusCode.OK, IsAnonimous = true };
    public static ProxyStatus Cancelled => new() { IsCancelled = true };

    public HttpStatusCode Status { get; private init; }

    public bool IsAnonimous { get; private init; }
    public bool IsCancelled { get; private init; }

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