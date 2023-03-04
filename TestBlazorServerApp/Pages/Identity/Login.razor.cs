using Application.Constants.Identity;
using Application.Services.Identity;
using Application.Services.Lifecycle;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Pages.Identity;

public partial class Login
{
    [Inject] private IRunningServerState ServerState { get; set; } = null!;
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private UserManager<AppUserDb> UserManager { get; set; } = null!;

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
            var authResponse = await AccountService.LoginAsync(new UserLoginRequest
            {
                Username = Username,
                Password = Password
            });

            if (!authResponse.Succeeded)
            {
                authResponse.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
                return;
            }
        
            // TODO: Handle Auth state provider and httpclient authr bearer header updates for user auth
            Token = authResponse.Data.Token;
            RefreshToken = authResponse.Data.RefreshToken;
            Expiration = authResponse.Data.RefreshTokenExpiryTime;
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
            NavManager.NavigateTo("/register");
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
}