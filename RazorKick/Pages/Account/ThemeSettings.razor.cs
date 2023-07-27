﻿using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity.User;
using Domain.Enums.Identity;
using Domain.Models.Identity;
using MudBlazor.Utilities;
using RazorKick.Settings;

namespace RazorKick.Pages.Account;

public partial class ThemeSettings
{
    [CascadingParameter] public MainLayout ParentLayout { get; set; } = null!;

    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    private AppUserPreferenceFull _userPreferences = new();
    private AppUserFull CurrentUser { get; set; } = new();
    private AppThemeCustom _editingTheme = AppThemeCustom.GetExampleCustomOne();
    private AppThemeId _editingThemeId = AppThemeId.CustomOne;
    private bool _canEditTheme;
    
    private MudColor _editThemePrimaryColor = new("#FFFFFF");
    private MudColor _editThemeSecondaryColor = new("#FFFFFF");
    private MudColor _editThemeTertiaryColor = new("#FFFFFF");
    private MudColor _editThemeBackgroundColor = new("#FFFFFF");
    private MudColor _editThemeTitleBarColor = new("#FFFFFF");
    private MudColor _editThemeNavBarColor = new("#FFFFFF");
    private MudColor _editThemeSuccessColor = new("#FFFFFF");
    private MudColor _editThemeErrorColor = new("#FFFFFF");
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            await GetPermissions();
            await GetPreferences();
        }
    }

    private async Task GetCurrentUser()
    {
        var userId = await CurrentUserService.GetCurrentUserId();
        if (userId is null)
            return;

        CurrentUser = (await UserService.GetByIdFullAsync((Guid) userId)).Data!;
    }

    private async Task GetPermissions()
    {
        var currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _canEditTheme = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Preferences.ChangeTheme);
    }

    private async Task GetPreferences()
    {
        var preferences = await AccountService.GetPreferences(CurrentUser.Id);
        if (!preferences.Succeeded)
        {
            preferences.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }

        _userPreferences = preferences.Data;
        UpdateEditingThemeValues();
    }

    private void UpdateEditingThemeValues(AppThemeId themeId = AppThemeId.CustomOne)
    {
        _editingTheme = AppThemes.GetPreferenceCustomThemeFromId(_userPreferences, themeId);
        _editingThemeId = themeId;

        _editThemePrimaryColor = new MudColor(_editingTheme.ColorPrimary);
        _editThemeSecondaryColor = new MudColor(_editingTheme.ColorSecondary);
        _editThemeTertiaryColor = new MudColor(_editingTheme.ColorTertiary);
        _editThemeBackgroundColor = new MudColor(_editingTheme.ColorBackground);
        _editThemeTitleBarColor = new MudColor(_editingTheme.ColorTitleBar);
        _editThemeNavBarColor = new MudColor(_editingTheme.ColorNavBar);
        _editThemeSuccessColor = new MudColor(_editingTheme.ColorSuccess);
        _editThemeErrorColor = new MudColor(_editingTheme.ColorError);
        
        StateHasChanged();
    }

    private async Task SavePreferences()
    {
        var updatePreferences = _userPreferences.ToUpdate();
        var requestResult = await UserService.UpdatePreferences(CurrentUser.Id, updatePreferences);
        if (!requestResult.Succeeded)
        {
            requestResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }

        Snackbar.Add("Themes successfully updated!");
        await GetPreferences();
        StateHasChanged();
    }

    private void ResetSelectedThemeToDefault()
    {
        switch (_editingThemeId)
        {
            case AppThemeId.CustomOne:
                _userPreferences.CustomThemeOne = AppThemeCustom.GetExampleCustomOne();
                break;
            case AppThemeId.CustomTwo:
                _userPreferences.CustomThemeTwo = AppThemeCustom.GetExampleCustomTwo();
                break;
            case AppThemeId.CustomThree:
                _userPreferences.CustomThemeThree = AppThemeCustom.GetExampleCustomThree();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_editingThemeId));
        }
        
        UpdateEditingThemeValues(_editingThemeId);
    }
}