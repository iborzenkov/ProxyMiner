namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy authorization data.
/// </summary>
public sealed class ProxyAuthorizationData : IEquatable<ProxyAuthorizationData>
{
    /// <summary>
    ///     Initializes a new instance of the ProxyAuthorizationData class.
    /// </summary>
    /// <param name="username">The username for proxy authentication.</param>
    /// <param name="password">The password for proxy authentication. Can be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when username is null or whitespace.</exception>
    public ProxyAuthorizationData(string username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException(nameof(username));

        Username = username;
        Password = password;
    }
    
    /// <summary>
    ///     Username.
    /// </summary>
    public string Username { get; }
    
    /// <summary>
    ///     Password.
    /// </summary>
    public string? Password { get; }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Username, Password);
    }

    public bool Equals(ProxyAuthorizationData? other)
    {
        if (other is null) 
            return false;
        
        if (ReferenceEquals(this, other)) 
            return true;

        var isPasswordEquals = Password == null && other.Password == null 
            || Password != null && Password.Equals(other.Password, StringComparison.Ordinal);
        return isPasswordEquals 
            && Username.Equals(other.Username, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is ProxyAuthorizationData other && Equals(other);
}