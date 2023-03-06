using System.ComponentModel.DataAnnotations;
using Application.Constants.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;
using Bcrypt = BCrypt.Net.BCrypt;

namespace Application.Helpers.Identity;

public static class AccountHelpers
{
    public static string GenerateSalt()
    {
        return Bcrypt.GenerateSalt();
    }

    public static string GenerateHash(string password, string salt)
    {
        return Bcrypt.HashPassword(password, salt);
    }

    public static void GenerateHashAndSalt(string password, out string salt, out string hash)
    {
        salt = GenerateSalt();
        hash = GenerateHash(password, salt);
    }

    public static bool IsPasswordCorrect(string password, string salt, string hash)
    {
        var newHash = GenerateHash(password, salt);
        return hash == newHash;
    }

    public static string NormalizeForDatabase(this string providedString)
    {
        return providedString.Normalize();
    }

    public static bool IsValidEmailAddress(string? address)
    {
        return address != null && new EmailAddressAttribute().IsValid(address);
    }

    public static async Task<bool> PasswordMeetsRequirements(string password)
    {
        var validPassword = await new PasswordValidator<AppUserDb>().ValidateAsync(null, new AppUserDb(), password);
        return validPassword.Succeeded;
    }
}