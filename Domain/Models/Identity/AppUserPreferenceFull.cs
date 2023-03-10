using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Newtonsoft.Json;

namespace Domain.Models.Identity;

public class AppUserPreferenceFull
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public AppThemeId ThemePreference { get; set; } = AppThemeId.Dark;
    public bool DrawerDefaultOpen { get; set; } = false;
    public AppThemeCustom CustomThemeOne { get; set; } = new();
    public AppThemeCustom CustomThemeTwo { get; set; } = new();
    public AppThemeCustom CustomThemeThree { get; set; } = new();
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
            CustomThemeOne = JsonConvert.DeserializeObject<AppThemeCustom>(preferenceDb.CustomThemeOne),
            CustomThemeTwo = JsonConvert.DeserializeObject<AppThemeCustom>(preferenceDb.CustomThemeTwo),
            CustomThemeThree = JsonConvert.DeserializeObject<AppThemeCustom>(preferenceDb.CustomThemeTwo)
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
            CustomThemeOne = JsonConvert.SerializeObject(preferenceFull.CustomThemeOne),
            CustomThemeTwo = JsonConvert.SerializeObject(preferenceFull.CustomThemeTwo),
            CustomThemeThree = JsonConvert.SerializeObject(preferenceFull.CustomThemeThree)
        };
    }
}