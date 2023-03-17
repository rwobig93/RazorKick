using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Application.Constants.Identity;

public static class UserConstants
{
    public const string DefaultAdminUsername = "Superperson";
    public const string DefaultAdminFirstName = "Admini";
    public const string DefaultAdminLastName = "Strator";
    public const string DefaultAdminEmail = "Superperson@localhost.local";
    public const string DefaultAdminPassword = "^8yi#aFU9GstJdU9PwK9b6!&t^6hyjUg3!v^FT2cDF5mwjPGyvwfiR*";
    
    public const string DefaultBasicUsername = "TheNeutral";
    public const string DefaultBasicFirstName = "Neutral";
    public const string DefaultBasicLastName = "Maybe";
    public const string DefaultBasicEmail = "TheNeutral@doop.future";
    public const string DefaultBasicPassword = "wFWHo^^@Lv%df$Exo7h&KWeTj35t4g3GBu^LPz9^35KCDT6A@K#zMZ3";
    
    public const string DefaultSystemUsername = "System";
    public const string DefaultSystemFirstName = "The";
    public const string DefaultSystemLastName = "System";
    public const string DefaultSystemEmail = "TheSystem@localhost.local";
    
    public const string DefaultAnonymousUsername = "Anonymous";
    public const string DefaultAnonymousFirstName = "Anonymous";
    public const string DefaultAnonymousLastName = "User";
    public const string DefaultAnonymousEmail = "Who@am.i";

    public static PasswordOptions PasswordRequirements = new()
    {
        RequiredLength = 12,
        RequiredUniqueChars = 1,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true
    };

    public static UserOptions UserRequirements = new()
    {
        AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._$@+",
        RequireUniqueEmail = true
    };

    public static ClaimsIdentity UnauthenticatedIdentity = new();

    public static ClaimsPrincipal UnauthenticatedPrincipal = new(UnauthenticatedIdentity);
}