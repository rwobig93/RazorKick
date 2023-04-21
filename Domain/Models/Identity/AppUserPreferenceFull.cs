using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;

namespace Domain.Models.Identity;

public class AppUserPreferenceFull
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public AppThemeId ThemePreference { get; set; } = AppThemeId.Dark;
    public bool DrawerDefaultOpen { get; set; }
    public AppThemeCustom CustomThemeOne { get; set; } = AppThemeCustom.GetExampleCustomOne();
    public AppThemeCustom CustomThemeTwo { get; set; } = AppThemeCustom.GetExampleCustomTwo();
    public AppThemeCustom CustomThemeThree { get; set; } = AppThemeCustom.GetExampleCustomThree();
}

public static class AppUserPreferenceFullExtensions
{
    public static AppUserPreferenceFull ToFull(this AppUserPreferenceDb preferenceDb)
    {
        return new AppUserPreferenceFull
        {
            Id = preferenceDb.Id,
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = AppThemeCustom.GetExampleCustomOne(),
            CustomThemeTwo = AppThemeCustom.GetExampleCustomTwo(),
            CustomThemeThree = AppThemeCustom.GetExampleCustomThree()
        };
    }

    public static AppUserPreferenceDb ToDb(this AppUserPreferenceFull preferenceFull)
    {
        return new AppUserPreferenceDb
        {
            Id = preferenceFull.Id,
            OwnerId = preferenceFull.OwnerId,
            ThemePreference = preferenceFull.ThemePreference,
            DrawerDefaultOpen = preferenceFull.DrawerDefaultOpen,
            CustomThemeOne = string.Empty,
            CustomThemeTwo = string.Empty,
            CustomThemeThree = string.Empty
        };
    }
}