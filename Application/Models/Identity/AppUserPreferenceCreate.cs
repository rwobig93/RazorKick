using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;
using Newtonsoft.Json;

namespace Application.Models.Identity;

public class AppUserPreferenceCreate
{
    public Guid OwnerId { get; set; }
    public AppThemeId ThemePreference { get; set; } = AppThemeId.Dark;
    public bool DrawerDefaultOpen { get; set; } = true;
    public string? CustomThemeOne { get; set; } = JsonConvert.SerializeObject(new AppThemeCustom());
    public string? CustomThemeTwo { get; set; } = JsonConvert.SerializeObject(new AppThemeCustom());
    public string? CustomThemeThree { get; set; } = JsonConvert.SerializeObject(new AppThemeCustom());
}

public static class AppUserPreferenceCreateExtensions
{
    public static AppUserPreferenceCreate ToCreate(this AppUserPreferenceDb preferenceDb)
    {
        return new AppUserPreferenceCreate
        {
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = preferenceDb.CustomThemeOne,
            CustomThemeTwo = preferenceDb.CustomThemeTwo,
            CustomThemeThree = preferenceDb.CustomThemeThree
        };
    }

    public static AppUserPreferenceCreate ToCreate(this AppUserPreferenceUpdate preferenceUpdate)
    {
        return new AppUserPreferenceCreate
        {
            ThemePreference = preferenceUpdate.ThemePreference,
            DrawerDefaultOpen = preferenceUpdate.DrawerDefaultOpen,
            CustomThemeOne = preferenceUpdate.CustomThemeOne,
            CustomThemeTwo = preferenceUpdate.CustomThemeTwo,
            CustomThemeThree = preferenceUpdate.CustomThemeThree
        };
    }

    public static AppUserPreferenceDb ToDb(this AppUserPreferenceCreate preferenceCreate)
    {
        return new AppUserPreferenceDb
        {
            OwnerId = preferenceCreate.OwnerId,
            ThemePreference = preferenceCreate.ThemePreference,
            DrawerDefaultOpen = preferenceCreate.DrawerDefaultOpen,
            CustomThemeOne = preferenceCreate.CustomThemeOne,
            CustomThemeTwo = preferenceCreate.CustomThemeTwo,
            CustomThemeThree = preferenceCreate.CustomThemeThree
        };
    }
}