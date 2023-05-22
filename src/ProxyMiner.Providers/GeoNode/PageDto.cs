namespace ProxyMiner.Providers.GeoNode;

internal sealed class PageDto
{
    public List<ProxyDto>? Data { get; set; }
    public int? Total { get; set; }
    public int? Page { get; set; }
    public int? Limit { get; set; }
}