using Application.Services.Identity;
using Application.Services.System;
using Microsoft.AspNetCore.Components;
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Pages.Identity;

public partial class Register
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;

    private string DesiredUsername { get; set; } = "";
    private string DesiredEmail { get; set; } = "";
    private string DesiredPassword { get; set; } = "";
    private string ConfirmPassword { get; set; } = "";
    
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private InputType _passwordConfirmInput = InputType.Password;
    private string _passwordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool PageIsLoading { get; set; } = false;

    private async Task RegisterAsync()
    {
        try
        {
            if (!IsRequiredInformationPresent()) return;

            PageIsLoading = true;
            StateHasChanged();

            var authResponse = await AccountService.RegisterAsync(new UserRegisterRequest
            {
                Username = DesiredUsername,
                Email = DesiredEmail,
                Password = DesiredPassword
            });

            if (!authResponse.Succeeded)
            {
                authResponse.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
                return;
            }

            authResponse.Messages.ForEach(x => Snackbar.Add(x, Severity.Success));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failure Occurred: {ex.Message}", Severity.Error);
        }
        finally
        {
            PageIsLoading = false;
            StateHasChanged();
        }
    }

    private bool IsRequiredInformationPresent()
    {
        var informationValid = true;
        
        if (string.IsNullOrWhiteSpace(DesiredUsername)) {
            Snackbar.Add("Username field is empty", Severity.Error); informationValid = false; }
        if (string.IsNullOrWhiteSpace(DesiredEmail)) {
            Snackbar.Add("Email field is empty", Severity.Error); informationValid = false; }
        if (string.IsNullOrWhiteSpace(DesiredPassword)) {
            Snackbar.Add("Password field is empty", Severity.Error); informationValid = false; }
        if (string.IsNullOrWhiteSpace(ConfirmPassword)) {
            Snackbar.Add("ConfirmPassword field is empty", Severity.Error); informationValid = false; }
        if (DesiredPassword != ConfirmPassword) {
            Snackbar.Add("Passwords provided don't match", Severity.Error); informationValid = false; }

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

    private void DebugFillRegistrationInfo()
    {
        DesiredUsername = "wingman";
        DesiredEmail = "wingman@wobigtech.net";
        DesiredPassword = "MZB*R*odX%Hy!J6b7Jrm6fK@PJ77v*dfWB$*9PtUV%zPTetSb!VP!uk";
        ConfirmPassword = "MZB*R*odX%Hy!J6b7Jrm6fK@PJ77v*dfWB$*9PtUV%zPTetSb!VP!uk";
    }
}