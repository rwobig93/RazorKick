using System.Security.Claims;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using Shared.Responses.Identity;

namespace TestBlazorServerApp.Pages.Account;

public partial class AccountSettings
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    private AppUserPreferenceFull _userPreferences = new();
    private AppUserFull CurrentUser { get; set; } = new();
    
    
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
        var userId = await CurrentUserService.GetCurrentUserId();
        if (userId is null)
            return;

        CurrentUser = (await UserRepository.GetByIdFullAsync((Guid) userId)).Result!;
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
    }

    private async Task UpdateAccount()
    {
        var updatedAccount = CurrentUser.ToUpdateObject();
        var requestResult = await UserRepository.UpdateAsync(updatedAccount);
        if (!requestResult.Success)
        {
            Snackbar.Add(requestResult.ErrorMessage, Severity.Error);
            return;
        }

        Snackbar.Add("Account successfully updated!");
        StateHasChanged();
    }
}