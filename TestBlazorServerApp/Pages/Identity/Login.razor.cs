using System.Net.Http.Headers;
using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Repositories.Identity;
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
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Pages.Identity;

public partial class Login
{
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;

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
            if (string.IsNullOrWhiteSpace(Username)) Snackbar.Add("Username field is empty", Severity.Error);
            if (string.IsNullOrWhiteSpace(Password)) Snackbar.Add("Password field is empty", Severity.Error);
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) return;
            
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
    
    private void DebugFillAdminCreds()
    {
        Username = UserConstants.DefaultAdminUsername;
        Password = UserConstants.DefaultAdminPassword;
    }
    
    private void DebugFillBasicCreds()
    {
        Username = UserConstants.DefaultBasicUsername;
        Password = UserConstants.DefaultBasicPassword;
    }
}