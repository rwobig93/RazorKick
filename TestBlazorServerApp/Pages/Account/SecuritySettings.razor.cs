using Application.Constants.Web;
using Application.Helpers.Identity;
using Application.Models.Identity;
using Application.Responses.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Pages.Account;

public partial class SecuritySettings
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IQrCodeService QrCodeService { get; init; } = null!;
    [Inject] private IMfaService MfaService { get; init; } = null!;

    private AppUserFull CurrentUser { get; set; } = new();

    // User Password Change
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

    // MFA
    private string _mfaButtonText = "";
    private string _mfaRegisterCode = "";
    private string _qrCodeImageSource = "";
    private string _totpCode = "";
    private bool QrCodeGenerating { get; set; }
    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            UpdatePageElementStates();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUser()
    {
        var foundUser = await CurrentUserService.GetCurrentUserFull();
        if (foundUser is null)
            return;

        CurrentUser = foundUser;
    }

    private void UpdatePageElementStates()
    {
        _mfaButtonText = CurrentUser.TwoFactorEnabled switch
        {
            true => "Disable MFA",
            false when !string.IsNullOrWhiteSpace(CurrentUser.TwoFactorKey) => "Enable MFA",
            _ => "Register MFA TOTP Token"
        };
    }

    private async Task UpdatePassword()
    {
        if (!await IsRequiredInformationPresent())
            return;

        await AccountService.SetUserPassword(CurrentUser.Id, DesiredPassword);

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

        if (string.IsNullOrWhiteSpace(CurrentPassword))
        {
            Snackbar.Add("Current Password field is empty", Severity.Error);
            informationValid = false;
        }

        if (string.IsNullOrWhiteSpace(DesiredPassword))
        {
            Snackbar.Add("Desired Password field is empty", Severity.Error);
            informationValid = false;
        }

        if (string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            Snackbar.Add("Confirm Password field is empty", Severity.Error);
            informationValid = false;
        }

        if (DesiredPassword != ConfirmPassword)
        {
            Snackbar.Add("Passwords provided don't match", Severity.Error);
            informationValid = false;
        }

        if (!(await AccountService.IsPasswordCorrect(CurrentUser.Id, CurrentPassword)).Data)
        {
            Snackbar.Add("Current password provided is incorrect", Severity.Error);
            informationValid = false;
        }

        if (!(await AccountService.PasswordMeetsRequirements(DesiredPassword)).Data)
        {
            Snackbar.Add("Desired password doesn't meet the password requirements", Severity.Error);
            informationValid = false;
        }

        return informationValid;
    }

    private async Task InvokeTotpAction()
    {
        // If the account doesn't have a MFA key we want to allow registering
        if (string.IsNullOrWhiteSpace(CurrentUser.TwoFactorKey))
        {
            await RegisterTotp();
            return;
        }

        // If we have a MFA key on the account we want to allow toggling MFA on/off for the account
        await ToggleMfaEnablement(!CurrentUser.TwoFactorEnabled);
    }
    
    private async Task RegisterTotp()
    {
        QrCodeGenerating = true;
        await Task.Yield();
        
        try
        {
            _mfaRegisterCode = MfaService.GenerateKeyString();
            var appName = ServerState.ApplicationName;
            var qrCodeContent =
                MfaService.GenerateOtpAuthString(appName, CurrentUser.EmailAddress!, _mfaRegisterCode);
            _qrCodeImageSource = QrCodeService.GenerateQrCodeSrc(qrCodeContent);
            
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failed to generate TOTP Registration: {ex.Message}", Severity.Error);
        }
        
        QrCodeGenerating = false;
        await Task.CompletedTask;
    }

    private async Task ToggleMfaEnablement(bool enabled)
    {
        var result = await AccountService.SetTwoFactorEnabled(CurrentUser.Id, enabled);
        if (!result.Succeeded)
        {
            result.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }
        
        await GetCurrentUser();
        StateHasChanged();
        var mfaEnablement = CurrentUser.TwoFactorEnabled ? "Enabled" : "Disabled";
        Snackbar.Add($"Successfully toggled MFA to {mfaEnablement}");
        UpdatePageElementStates();
    }

    private async Task ValidateTotpCode()
    {
        // 56172502 | 3.11.28 AM CT, Saturday May 27th
        var totpCorrect = MfaService.IsPasscodeCorrect(_totpCode, _mfaRegisterCode, out var timeStampMatched);
        if (!totpCorrect)
        {
            Snackbar.Add("TOTP code provided is incorrect, please try again", Severity.Error);
            return;
        }
        
        Snackbar.Add("TOTP code provided is correct!", Severity.Success);
        // TODO: Remove when done testing / verifying matching timestamp for TOTP used validation
        Snackbar.Add($"Matched Timestamp for {CurrentUser.Username}: {timeStampMatched}", Severity.Info);
        Logger.Information("Matched Timestamp for {Username}: {MfaTimestampMatched}", CurrentUser.Username, timeStampMatched);
        await AccountService.SetTwoFactorKey(CurrentUser.Id, _mfaRegisterCode);

        _mfaRegisterCode = "";
        _qrCodeImageSource = "";
        await AccountService.SetTwoFactorEnabled(CurrentUser.Id, true);
        NavManager.NavigateTo(AppRouteConstants.Account.Security, true);
    }
}