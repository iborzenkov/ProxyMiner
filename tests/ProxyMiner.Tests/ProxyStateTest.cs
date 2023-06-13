using ProxyMiner.Core.Models;

namespace ProxyMiner.Tests;

[TestClass]
public class ProxyStateTest
{
    [TestMethod]
    public void ProxyState_StartChecking_MinValue()
    {
        var date = DateTime.MinValue;
        var state = ProxyState.StartChecking(date);
        Assert.IsNotNull(state);
        Assert.AreEqual(date, state.StartTimeUtc);
        Assert.IsNull(state.FinishTimeUtc);
        Assert.IsNull(state.Status);
    }

    [TestMethod]
    public void ProxyState_StartChecking_FromFeature()
    {
        var date = DateTime.UtcNow.AddYears(5);
        var state = ProxyState.StartChecking(date);
        Assert.IsNotNull(state);
        Assert.AreEqual(date, state.StartTimeUtc);
        Assert.IsNull(state.FinishTimeUtc);
        Assert.IsNull(state.Status);
    }

    [TestMethod]
    public void ProxyState_NotDefined()
    {
        var state = ProxyState.NotDefined;
        Assert.IsNotNull(state);
        Assert.IsNull(state.StartTimeUtc);
        Assert.IsNull(state.FinishTimeUtc);
        Assert.IsNull(state.Status);
    }

    [TestMethod]
    public void ProxyState_Regular()
    {
        var start = DateTime.UtcNow.AddHours(-3);
        var finish = DateTime.UtcNow;
        var status = ProxyStatus.Cancelled;
        var state = new ProxyState(start, finish, status);
        Assert.IsNotNull(state);
        Assert.AreEqual(start, state.StartTimeUtc);
        Assert.AreEqual(finish, state.FinishTimeUtc);
        Assert.AreEqual(status, state.Status);
    }

    [TestMethod]
    public void ProxyState_FinishEqualStart()
    {
        var start = DateTime.UtcNow;
        var finish = start;
        var status = ProxyStatus.Cancelled;
        var state = new ProxyState(start, finish, status);
        Assert.IsNotNull(state);
        Assert.AreEqual(start, state.StartTimeUtc);
        Assert.AreEqual(finish, state.FinishTimeUtc);
        Assert.AreEqual(status, state.Status);
    }

    [TestMethod]
    public void ProxyState_FinishGreaterThenStart()
    {
        var start = DateTime.UtcNow;
        var finish = start.AddDays(-2);
        var status = ProxyStatus.Cancelled;
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new ProxyState(start, finish, status));
    }
}