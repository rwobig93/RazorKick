using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Security.Claims;
using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Auth;
using Application.Helpers.Identity;
using Application.Helpers.Web;
using Application.Repositories.Identity;
using Application.Requests.Identity.User;
using Application.Services.Identity;
using Application.Services.Integrations;
using Application.Services.System;
using Application.Settings.AppSettings;
using Blazored.LocalStorage;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Enums.Integration;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using OAuth2.Client;
using OAuth2.Client.Impl;
using OAuth2.Infrastructure;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Identity;

public partial class Login
{
    [Parameter] public string RedirectReason { get; set; } = "";
    [Parameter] public string OauthCode { get; set; } = "";
    [Parameter] public string OauthState { get; set; } = "";
    
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    [Inject] private IOptions<AppConfiguration> AppSettings { get; init; } = null!;
    [Inject] private IOptions<OauthConfiguration> OauthSettings { get; init; } = null!;
    [Inject] private IExternalAuthProviderService ExternalAuth { get; set; } = null!;

    private string Username { get; set; } = "";
    private string Password { get; set; } = "";
    private string Token { get; set; } = "";
    private string RefreshToken { get; set; } = "";
    private DateTime Expiration { get; set; }
    private bool ShowAuth { get; set; } = false;
    private List<string> AuthResults { get; set; } = new();
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ParseParametersFromUri();
            HandleRedirectReasons();
            await HandleExternalLoginRedirect();
            StateHasChanged();
        }

        await Task.CompletedTask;
    }

    private void ParseParametersFromUri()
    {
        var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);
        
        if (queryParameters.TryGetValue(LoginRedirectConstants.RedirectParameter, out var redirectReason))
            RedirectReason = redirectReason!;
        
        if (queryParameters.TryGetValue(LoginRedirectConstants.OauthCode, out var oauthCode))
            OauthCode = oauthCode!;
        
        if (queryParameters.TryGetValue(LoginRedirectConstants.OauthState, out var oauthState))
            OauthState = oauthState!;
    }

    private void HandleRedirectReasons()
    {
        if (string.IsNullOrWhiteSpace(RedirectReason))
            return;

        switch (RedirectReason)
        {
            case nameof(LoginRedirectReason.SessionExpired):
                Snackbar.Add(LoginRedirectConstants.SessionExpired, Severity.Error);
                break;
            case nameof(LoginRedirectReason.ReAuthenticationForce):
                Snackbar.Add(LoginRedirectConstants.ReAuthenticationForce, Severity.Error);
                break;
            case nameof(LoginRedirectReason.FullLoginTimeout):
                Snackbar.Add(LoginRedirectConstants.FullLoginTimeout, Severity.Error);
                break;
            default:
                Snackbar.Add(LoginRedirectConstants.Unknown, Severity.Error);
                break;
        }
    }

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
            
            NavManager.NavigateTo(AppSettings.Value.BaseUrl, true);
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

        var foundUser = await UserService.GetByUsernameSecurityFullAsync(Username);
        if (!foundUser.Succeeded || foundUser.Data is null)
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

        var mfaResponse = await DialogService.ShowAsync<MfaCodeValidationDialog>("MFA Token Validation", dialogParameters, dialogOptions);
        var mfaTokenValid = await mfaResponse.Result;
        return !mfaTokenValid.Canceled;
    }

    private async Task InitiateExternalLogin(ExternalAuthProvider provider)
    {
        var providerLoginUriRequest = await ExternalAuth.GetLoginUri(provider);
        if (!providerLoginUriRequest.Succeeded || string.IsNullOrWhiteSpace(providerLoginUriRequest.Data))
        {
            Snackbar.Add($"Failed to initiate login to desired provider: [{provider.ToString()}]", Severity.Error);
            providerLoginUriRequest.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }
        
        NavManager.NavigateTo(providerLoginUriRequest.Data);
    }

    private async Task HandleExternalLoginRedirect()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(OauthCode) || string.IsNullOrWhiteSpace(OauthState)) return;

            var isValidProvider = Enum.TryParse(OauthState, out ExternalAuthProvider parsedProvider);
            if (!isValidProvider)
            {
                Snackbar.Add($"Provided provider redirect is not valid: {OauthState}", Severity.Error);
                return;
            }

            var externalProfileRequest = await ExternalAuth.GetUserProfile(parsedProvider, OauthCode);
            if (!externalProfileRequest.Succeeded)
            {
                Snackbar.Add("Provided Oauth code is invalid", Severity.Error);
                return;
            }

            Snackbar.Add($"Successfully authenticated with external provider! => {parsedProvider.ToString()}", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Failure occurred when attempting to handle external login redirect, likely invalid data provided");
            Snackbar.Add("Invalid external login data provided, please try logging in again", Severity.Error);
        }
    }
}