using System.Net;
using ProxyMiner.Core.Models;

namespace ProxyMiner.Tests;

[TestClass]
public class ProxyStatusTest
{
    [TestMethod]
    public void ProxyStatus_ErrorStatus()
    {
        var code = HttpStatusCode.BadGateway;
        var status = ProxyStatus.ErrorStatus(code);
        Assert.IsNotNull(status);
        Assert.AreEqual(code, status.Status);
        Assert.IsFalse(status.IsAnonimous);
        Assert.IsFalse(status.IsCancelled);
        Assert.IsFalse(status.IsTimeout);
        Assert.IsFalse(status.IsValid);
    }

    [TestMethod]
    public void ProxyStatus_NotAnonimous()
    {
        var status = ProxyStatus.NotAnonimous;
        Assert.IsNotNull(status);
        Assert.AreEqual(HttpStatusCode.OK, status.Status);
        Assert.IsFalse(status.IsAnonimous);
        Assert.IsFalse(status.IsCancelled);
        Assert.IsFalse(status.IsTimeout);
        Assert.IsTrue(status.IsValid);
    }

    [TestMethod]
    public void ProxyStatus_Anonimous()
    {
        var status = ProxyStatus.Anonimous;
        Assert.IsNotNull(status);
        Assert.AreEqual(HttpStatusCode.OK, status.Status);
        Assert.IsTrue(status.IsAnonimous);
        Assert.IsFalse(status.IsCancelled);
        Assert.IsFalse(status.IsTimeout);
        Assert.IsTrue(status.IsValid);
    }

    [TestMethod]
    public void ProxyStatus_Cancelled()
    {
        var status = ProxyStatus.Cancelled;
        Assert.IsNotNull(status);
        Assert.IsNull(status.Status);
        Assert.IsFalse(status.IsAnonimous);
        Assert.IsTrue(status.IsCancelled);
        Assert.IsFalse(status.IsTimeout);
        Assert.IsFalse(status.IsValid);
    }

    [TestMethod]
    public void ProxyStatus_Timeout()
    {
        var status = ProxyStatus.Timeout;
        Assert.IsNotNull(status);
        Assert.IsNull(status.Status);
        Assert.IsFalse(status.IsAnonimous);
        Assert.IsFalse(status.IsCancelled);
        Assert.IsTrue(status.IsTimeout);
        Assert.IsFalse(status.IsValid);
    }
}