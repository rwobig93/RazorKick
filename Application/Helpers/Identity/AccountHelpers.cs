using System.Net;
using System.Net.Mail;
using Application.Constants.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Responses.Identity;
using Bcrypt = BCrypt.Net.BCrypt;

namespace Application.Helpers.Identity;

public static class AccountHelpers
{
    public static PasswordValidator<AppUserDb> PasswordValidator { get; } = new();
    
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

    public static bool IsValidEmailAddress(string emailAddress, bool verifyHost = false)
    {
        try
        {
            var validEmail = new MailAddress(emailAddress);
            if (verifyHost)
            {
                Dns.GetHostEntry(validEmail.Host);
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static PasswordRequirementsResponse GetPasswordRequirements()
    {
        return new PasswordRequirementsResponse
        {
            MinimumLength = UserConstants.PasswordRequirements.RequiredLength,
            RequiresSpecialCharacters = UserConstants.PasswordRequirements.RequireNonAlphanumeric,
            RequiresLowercase = UserConstants.PasswordRequirements.RequireLowercase,
            RequiresUppercase = UserConstants.PasswordRequirements.RequireUppercase,
            RequiresNumbers = UserConstants.PasswordRequirements.RequireDigit
        };
    }

    private static bool PasswordContainsSpecialCharacter(this string password)
    {
        return (password.Any(char.IsPunctuation) || password.Any(char.IsSymbol) || password.Any(char.IsSeparator));
    }

    public static List<string> GetAnyIssuesWithPassword(string password)
    {
        var issueList = new List<string>();
        var passwordRequirements = GetPasswordRequirements();

        if (password.Length < passwordRequirements.MinimumLength)
            issueList.Add($"Password provided doesn't meet the minimum character count of {passwordRequirements.MinimumLength}");
        if (passwordRequirements.RequiresNumbers && !password.Any(char.IsDigit))
            issueList.Add("Password provided doesn't contain a number, which is a requirement");
        if (passwordRequirements.RequiresSpecialCharacters && !password.PasswordContainsSpecialCharacter())
            issueList.Add("Password provided doesn't contain a special character, which is a requirement");
        if (passwordRequirements.RequiresLowercase && !password.Any(char.IsLower))
            issueList.Add("Password provided doesn't contain a lowercase character, which is a requirement");
        if (passwordRequirements.RequiresUppercase && !password.Any(char.IsUpper))
            issueList.Add("Password provided doesn't contain an uppercase character, which is a requirement");

        return issueList;
    }

    public static bool DoesPasswordMeetRequirements(string password)
    {
        var passwordRequirements = GetPasswordRequirements();

        if (password.Length < passwordRequirements.MinimumLength)
            return false;
        if (passwordRequirements.RequiresNumbers && !password.Any(char.IsDigit))
            return false;
        if (passwordRequirements.RequiresSpecialCharacters && !password.PasswordContainsSpecialCharacter())
            return false;
        if (passwordRequirements.RequiresLowercase && !password.Any(char.IsLower))
            return false;
        if (passwordRequirements.RequiresUppercase && !password.Any(char.IsUpper))
            return false;

        return true;
    }
}