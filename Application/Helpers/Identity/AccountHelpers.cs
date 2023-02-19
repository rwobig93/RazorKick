using System.Security.Cryptography;

namespace Application.Helpers.Identity;

public static class AccountHelpers
{
    /// <summary>
    /// Get a password hash for the given string
    /// </summary>
    /// <param name="password">A (hopefully) secure string</param>
    /// <param name="salt">Random and securely generated salt for the generated hash</param>
    /// <param name="hash">Generated hash given the password and salt</param>
    public static void GetPasswordHash(string password, out byte[] salt, out byte[] hash)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
    
    /// <summary>
    /// Get a password hash for the given string
    /// </summary>
    /// <param name="password">A (hopefully) secure string</param>
    /// <param name="salt">Salt for the generated hash</param>
    /// <param name="hash">Generated hash given the password and salt</param>
    public static void GetPasswordHash(string password, byte[] salt, out byte[] hash)
    {
        using var hmac = new HMACSHA512(salt);
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    /// Indicates whether the provided password is correct given the salt and hash
    /// </summary>
    /// <param name="password">A (hopefully) secure string</param>
    /// <param name="salt">Salt for the generated hash</param>
    /// <param name="hash">Generated hash given the password and salt</param>
    /// <returns>Boolean indicating correct paassword</returns>
    public static bool IsPasswordCorrect(string password, byte[] salt, byte[] hash)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(hash);
    }
}