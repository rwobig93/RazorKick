using Application.Helpers.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Microsoft.AspNetCore.Components;
using Shared.Responses.Identity;

namespace TestBlazorServerApp.Pages.Account;

public partial class SecuritySettings
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    
    private Guid CurrentUserId { get; set; }
    private string CurrentPassword { get; set; } = "";
    private string DesiredPassword { get; set; } = "";
    private string ConfirmPassword { get; set; } = "";
    private readonly PasswordRequirementsResponse _passwordRequirements = AccountHelpers.GetPasswordRequirements();
    private InputType _passwordCurrentInput = InputType.Password;
    private string _passwordCurrentInputIcon = Icons.Material.Filled.VisibilityOff;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private InputType _passwordConfirmInput = InputType.Password;
    private string _passwordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUser()
    {
        var userId = await CurrentUserService.GetCurrentUserId();
        if (userId is null)
            return;

        CurrentUserId = (Guid)userId;
    }

    private async Task UpdatePassword()
    {
        if (!(await IsRequiredInformationPresent()))
            return;

        await AccountService.SetUserPassword(CurrentUserId, DesiredPassword);

        Snackbar.Add("Password successfully changed!");
        StateHasChanged();
    }

    private void ToggleCurrentPasswordVisibility()
    {
        if (_passwordCurrentInputIcon == Icons.Material.Filled.VisibilityOff)
        {
            _passwordCurrentInput = InputType.Text;
            _passwordCurrentInputIcon = Icons.Material.Filled.Visibility;
            return;
        }
        
        _passwordCurrentInput = InputType.Password;
        _passwordCurrentInputIcon = Icons.Material.Filled.VisibilityOff;
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordInputIcon == Icons.Material.Filled.VisibilityOff)
        {
            _passwordInput = InputType.Text;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            return;
        }
        
        _passwordInput = InputType.Password;
        _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    }

    private void ToggleConfirmPasswordVisibility()
    {
        if (_passwordConfirmInputIcon == Icons.Material.Filled.VisibilityOff)
        {
            _passwordConfirmInput = InputType.Text;
            _passwordConfirmInputIcon = Icons.Material.Filled.Visibility;
            return;
        }
        
        _passwordConfirmInput = InputType.Password;
        _passwordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;
    }

    private async Task<bool> IsRequiredInformationPresent()
    {
        var informationValid = true;
        
        if (string.IsNullOrWhiteSpace(CurrentPassword)) {
            Snackbar.Add("Current Password field is empty", Severity.Error); informationValid = false; }
        if (string.IsNullOrWhiteSpace(DesiredPassword)) {
            Snackbar.Add("Desired Password field is empty", Severity.Error); informationValid = false; }
        if (string.IsNullOrWhiteSpace(ConfirmPassword)) {
            Snackbar.Add("Confirm Password field is empty", Severity.Error); informationValid = false; }
        if (DesiredPassword != ConfirmPassword) {
            Snackbar.Add("Passwords provided don't match", Severity.Error); informationValid = false; }
        if (!(await AccountService.IsPasswordCorrect(CurrentUserId, CurrentPassword))) {
            Snackbar.Add("Current password provided is incorrect", Severity.Error); informationValid = false; }
        if (!AccountService.PasswordMeetsRequirements(DesiredPassword)) {
            Snackbar.Add("Desired password doesn't meet the password requirements", Severity.Error); informationValid = false; }
        
        return informationValid;
    }
}