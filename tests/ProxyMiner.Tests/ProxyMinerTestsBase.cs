using Moq;
using ProxyMiner.Core;
using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Options;

namespace ProxyMiner.Tests;

public abstract class ProxyMinerTestsBase
{
    protected ProxyMinerTestsBase()
    {
        Miner = MinerFactory.GetMiner(Checker, SettingsProvider);
    }

    [TestCleanup]
    public virtual void Cleanup()
    {
        Miner.Dispose();
    }

    protected void StartMiner()
    {
        Miner.Start();
        Thread.Sleep(1000);
    }

    protected void StopMiner()
    {
        Miner.Stop();
        Thread.Sleep(1000);
    }

    protected IMiner Miner { get; private set; }

    protected IChecker Checker => _checker ??= GetChecker();

    protected ISettingsProvider SettingsProvider => _settingsProvider ??= GetSettingsProvider();

    protected virtual Settings GetMockedSettings()
    {
        return new Settings
        {
            CheckThreadCount = 1,

            SourceTimeout = TimeSpan.FromSeconds(10),
            CheckTimeout = TimeSpan.FromSeconds(10),

            SourceScanPeriod = TimeSpan.FromMinutes(1),
            ExpiredProxyActualState = TimeSpan.FromMinutes(1)
        };
    }

    protected virtual IChecker GetChecker()
    {
        var checkerMock = new Mock<IChecker>();
        checkerMock.Setup(c => c.Check(It.IsAny<Proxy>(), It.IsAny<CancellationToken>()))
            .Returns(GetCheck());

        return checkerMock.Object;
    }

    protected virtual Task<ProxyStatus> GetCheck()
    {
        return Task.FromResult(ProxyStatus.Anonimous);
    }

    private ISettingsProvider GetSettingsProvider()
    {
        var settingsMock = new Mock<ISettingsProvider>();
        settingsMock.Setup(p => p.Settings)
            .Returns(GetMockedSettings());
        
        return settingsMock.Object;
    }

    private IChecker? _checker;
    private ISettingsProvider? _settingsProvider;
}
