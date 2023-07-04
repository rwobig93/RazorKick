using System.Net.Http.Headers;
using System.Security.Claims;
using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Repositories.Identity;
using Application.Requests.Identity.User;
using Application.Services.Identity;
using Application.Services.System;
using Blazored.LocalStorage;
using Domain.DatabaseEntities.Identity;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Identity;

public partial class Login
{
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserService UserService { get; set; } = null!;

    private string Username { get; set; } = "";
    private string Password { get; set; } = "";
    private string Token { get; set; } = "";
    private string RefreshToken { get; set; } = "";
    private DateTime Expiration { get; set; }
    private bool ShowAuth { get; set; } = false;
    private List<string> AuthResults { get; set; } = new();
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private async Task LoginAsync()
    {
        try
        {
            var loginReady = await IsLoginInfoAndMfaValid();
            if (!loginReady) return;
            
            var authResponse = await AccountService.LoginGuiAsync(new UserLoginRequest
            {
                Username = Username,
                Password = Password
            });
            
            if (!authResponse.Succeeded)
            {
                authResponse.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
                return;
            }
            
            Snackbar.Add("You're logged in, welcome to the party!", Severity.Success);
            
            NavManager.NavigateTo(NavManager.Uri, true);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failure Occurred: {ex.Message}", Severity.Error);
        }
    }

    private void ForgotPassword()
    {
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };

        DialogService.Show<ForgotPasswordDialog>("Forgot Password", dialogOptions);
    }

    private bool IsRequiredInformationPresent()
    {
        var informationValid = true;
        
        if (string.IsNullOrWhiteSpace(Username)) {
            Snackbar.Add("Username field is empty", Severity.Error); informationValid = false;
        }
        if (string.IsNullOrWhiteSpace(Password)) {
            Snackbar.Add("Password field is empty", Severity.Error); informationValid = false;
        }

        return informationValid;
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

    private void RegisterAsync()
    {
        try
        {
            NavManager.NavigateTo(AppRouteConstants.Identity.Register);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failure Occurred: {ex.Message}", Severity.Error);
        }
    }

    private void ToggleAuth()
    {
        ShowAuth = !ShowAuth;
    }
    
    private void DebugFillAdminCredentials()
    {
        Username = UserConstants.DefaultUsers.AdminUsername;
        Password = UserConstants.DefaultUsers.AdminPassword;
    }
    
    private void DebugFillModeratorCredentials()
    {
        Username = UserConstants.DefaultUsers.ModeratorUsername;
        Password = UserConstants.DefaultUsers.ModeratorPassword;
    }
    
    private void DebugFillBasicCredentials()
    {
        Username = UserConstants.DefaultUsers.BasicUsername;
        Password = UserConstants.DefaultUsers.BasicPassword;
    }

    private async Task<bool> IsLoginInfoAndMfaValid()
    {
        if (!IsRequiredInformationPresent()) return false;

        var foundUser = await UserService.GetByUsernameAsync(Username);
        if (!foundUser.Succeeded || foundUser.Data is null)
        {
            Snackbar.Add(ErrorMessageConstants.CredentialsInvalidError, Severity.Error);
            return false;
        }

        var passwordCorrect = await AccountService.IsPasswordCorrect(foundUser.Data.Id, Password);
        if (!passwordCorrect.Succeeded || !passwordCorrect.Data)
        {
            Snackbar.Add(ErrorMessageConstants.CredentialsInvalidError, Severity.Error);
            return false;
        }

        if (!foundUser.Data.TwoFactorEnabled) return true;
        
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium, CloseOnEscapeKey = true };
        var dialogParameters = new DialogParameters()
        {
            ["VerifyCodeMessage"]="Please enter your MFA code to login",
            ["MfaKey"]=foundUser.Data.TwoFactorKey
        };

        var mfaResponse = DialogService.Show<MfaCodeValidationDialog>("MFA Token Validation", dialogParameters, dialogOptions);
        var mfaTokenValid = await mfaResponse.Result;
        return !mfaTokenValid.Cancelled;
    }
}