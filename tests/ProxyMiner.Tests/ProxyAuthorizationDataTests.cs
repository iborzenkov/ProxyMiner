using ProxyMiner.Core.Models;

namespace ProxyMiner.Tests;

[TestClass]
public class ProxyAuthorizationDataTests
{
    [TestMethod]
    public void ProxyAuthorizationData_UsernameAndPasswordNotNull()
    {
        var username = "username";
        var password = "password";
        var authorizationData = new ProxyAuthorizationData(username, password);
        Assert.IsNotNull(authorizationData);
        Assert.AreEqual(username, authorizationData.Username);
        Assert.AreEqual(password, authorizationData.Password);
    }

    [TestMethod]
    public void ProxyAuthorizationData_UsernameIsNull()
    {
        string username = null!;
        var password = "password";
        Assert.ThrowsException<ArgumentNullException>(
            () => new ProxyAuthorizationData(username, password));
    }

    [TestMethod]
    public void ProxyAuthorizationData_UsernameIsEmpty()
    {
        string username = "   ";
        var password = "password";
        Assert.ThrowsException<ArgumentNullException>(
            () => new ProxyAuthorizationData(username, password));
    }

    [TestMethod]
    public void ProxyAuthorizationData_PasswordIsNull()
    {
        var username = "username";
        string password = null!;

        var authorizationData = new ProxyAuthorizationData(username, password);
        Assert.IsNotNull(authorizationData);
        Assert.AreEqual(username, authorizationData.Username);
        Assert.AreEqual(password, authorizationData.Password);
    }
}