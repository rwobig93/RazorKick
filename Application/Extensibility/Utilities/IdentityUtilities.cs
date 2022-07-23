using System.Security.Cryptography;

namespace Application.Extensibility.Utilities;

public static class IdentityUtilities
{
    public static void GetPasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
    
    public static void GetPasswordHash(string password, byte[] salt, out byte[] hash)
    {
        using var hmac = new HMACSHA512(salt);
        hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public static bool IsPasswordCorrect(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(hash);
    }
}