namespace ProxyMiner.Core.Models;

/// <summary>
///     Proxy authorization data.
/// </summary>
public sealed class ProxyAuthorizationData : IEquatable<ProxyAuthorizationData>
{
    public ProxyAuthorizationData(string username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("The username cannot be empty", nameof(username));

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
        if (ReferenceEquals(null, other)) 
            return false;
        
        if (ReferenceEquals(this, other)) 
            return true;

        var isPasswordEquals = Password == null && other.Password == null 
            || Password != null && Password.Equals(other.Password, StringComparison.Ordinal);
        return isPasswordEquals 
            && Username.Equals(other.Username, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) 
        => ReferenceEquals(this, obj) || obj is ProxyAuthorizationData other && Equals(other);
}