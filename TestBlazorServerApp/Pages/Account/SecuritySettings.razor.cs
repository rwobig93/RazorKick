using Application.Helpers.Identity;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using Shared.Responses.Identity;

namespace TestBlazorServerApp.Pages.Account;

public partial class SecuritySettings
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    
    private AppUserFull CurrentUser { get; set; } = new();
    private string CurrentPassword { get; set; } = "";
    private string DesiredPassword { get; set; } = "";
    private string ConfirmPassword { get; set; } = "";
    private readonly PasswordRequirementsResponse _passwordRequirements = AccountHelpers.GetPasswordRequirements();
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private InputType _passwordConfirmInput = InputType.Password;
    private string _passwordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;
    
    
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