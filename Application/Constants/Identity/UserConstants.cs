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

    public const string PathEmailTemplateConfirmation = "Infrastructure.EmailTemplates.RegistrationConfirmation.cshtml";
    public const string PathEmailTemplatePasswordReset = "Infrastructure.EmailTemplates.PasswordReset.cshtml";
}