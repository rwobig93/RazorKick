using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;
using Newtonsoft.Json;

namespace Application.Models.Identity;

public class AppUserPreferenceUpdate
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public AppThemeId ThemePreference { get; set; } = AppThemeId.Dark;
    public bool DrawerDefaultOpen { get; set; }
    public string? CustomThemeOne { get; set; }
    public string? CustomThemeTwo { get; set; }
    public string? CustomThemeThree { get; set; }
}

public static class AppUserPreferenceUpdateExtensions
{
    public static AppUserPreferenceDb ToDb(this AppUserPreferenceUpdate preferenceUpdate)
    {
        return new AppUserPreferenceDb
        {
            Id = Guid.Empty,
            OwnerId = preferenceUpdate.OwnerId,
            ThemePreference = preferenceUpdate.ThemePreference,
            DrawerDefaultOpen = preferenceUpdate.DrawerDefaultOpen,
            CustomThemeOne = preferenceUpdate.CustomThemeOne,
            CustomThemeTwo = preferenceUpdate.CustomThemeTwo,
            CustomThemeThree = preferenceUpdate.CustomThemeThree
        };
    }
    
    public static AppUserPreferenceUpdate ToUpdate(this AppUserPreferenceDb preferenceDb)
    {
        return new AppUserPreferenceUpdate
        {
            Id = preferenceDb.Id,
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = preferenceDb.CustomThemeOne,
            CustomThemeTwo = preferenceDb.CustomThemeTwo,
            CustomThemeThree = preferenceDb.CustomThemeThree
        };
    }
    
    public static AppUserPreferenceUpdate ToUpdate(this AppUserPreferenceFull preferenceDb)
    {
        return new AppUserPreferenceUpdate
        {
            Id = preferenceDb.Id,
            OwnerId = preferenceDb.OwnerId,
            ThemePreference = preferenceDb.ThemePreference,
            DrawerDefaultOpen = preferenceDb.DrawerDefaultOpen,
            CustomThemeOne = JsonConvert.SerializeObject(preferenceDb.CustomThemeOne),
            CustomThemeTwo = JsonConvert.SerializeObject(preferenceDb.CustomThemeTwo),
            CustomThemeThree = JsonConvert.SerializeObject(preferenceDb.CustomThemeThree)
        };
    }
}