using System.Security.Claims;
using Application.Services.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Pages.Account;

public partial class AccountSettings
{
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    private AppUserPreferenceFull _userPreferences = new();
    private ClaimsPrincipal CurrentUser { get; set; } = new();
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            await GetPreferences();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUser()
    {
        CurrentUser = await CurrentUserService.GetCurrentUserPrincipal() ?? new ClaimsPrincipal();
    }

    private async Task GetPreferences()
    {
        var userId = CurrentUserService.GetIdFromPrincipal(CurrentUser);
        var preferences = await AccountService.GetPreferences(userId);
        if (!preferences.Succeeded)
        {
            preferences.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }

        _userPreferences = preferences.Data;
    }
}