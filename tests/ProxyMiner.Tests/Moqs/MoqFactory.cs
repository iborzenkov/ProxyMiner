namespace ProxyMiner.Tests.Moqs;

internal static class MoqFactory
{
    public static CheckObserverBuilder CheckObserverBuilder => new();

    public static ProxyProviderBuilder ProxyProviderBuilder => new();
    
    public static CheckerBuilder CheckerBuilder => new();
}