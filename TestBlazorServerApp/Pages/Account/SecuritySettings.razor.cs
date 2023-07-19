using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Identity;
using Application.Helpers.Runtime;
using Application.Models.Identity.User;
using Application.Models.Identity.UserExtensions;
using Application.Responses.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.Enums.Auth;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace TestBlazorServerApp.Pages.Account;

public partial class SecuritySettings
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IQrCodeService QrCodeService { get; init; } = null!;
    [Inject] private IMfaService MfaService { get; init; } = null!;
    [Inject] private IWebClientService WebClientService { get; init; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;

    private AppUserSecurityFull CurrentUser { get; set; } = new();
    
    private bool _canGenerateApiTokens;
    
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
    
    // User API Tokens
    private List<AppUserExtendedAttributeSlim> _userApiTokens = new();
    private TimeZoneInfo _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT");
    private string _apiTokenGenButtonText = "Generate Token";
    private UserApiTokenTimeframe _tokenTimeframe = UserApiTokenTimeframe.OneYear;
    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            await GetPermissions();
            await GetClientTimezone();
            await GetUserApiTokens();
            UpdatePageElementStates();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUser()
    {
        var foundUser = await CurrentUserService.GetCurrentUserSecurityFull();
        if (foundUser is null)
            return;

        CurrentUser = foundUser;
    }

    private async Task GetPermissions()
    {
        var currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _canGenerateApiTokens = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Api.GenerateToken);
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
                MfaService.GenerateOtpAuthString(appName, CurrentUser.Email, _mfaRegisterCode);
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
        var totpCorrect = MfaService.IsPasscodeCorrect(_totpCode, _mfaRegisterCode, out _);
        if (!totpCorrect)
        {
            Snackbar.Add("TOTP code provided is incorrect, please try again", Severity.Error);
            return;
        }
        
        await AccountService.SetTwoFactorKey(CurrentUser.Id, _mfaRegisterCode);

        _mfaRegisterCode = "";
        _qrCodeImageSource = "";
        await AccountService.SetTwoFactorEnabled(CurrentUser.Id, true);
        Snackbar.Add("TOTP code provided is correct!", Severity.Success);
        
        // Wait for the snackbar message to be read then we reload the page to force page elements to update
        //  would love to find a better solution for this but as of now StateHasChanged or force updating doesn't work
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        NavManager.NavigateTo(AppRouteConstants.Account.Security, true);
    }

    private async Task TotpSubmitCheck(KeyboardEventArgs arg)
    {
        if (arg.Key == "Enter")
            await ValidateTotpCode();
    }

    private async Task GetClientTimezone()
    {
        var clientTimezoneRequest = await WebClientService.GetClientTimezone();
        if (!clientTimezoneRequest.Succeeded)
            clientTimezoneRequest.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));

        _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clientTimezoneRequest.Data);
    }

    private async Task GetUserApiTokens()
    {
        if (!_canGenerateApiTokens) return;

        var tokensRequest =
            await UserService.GetUserExtendedAttributesByTypeAsync(CurrentUser.Id, ExtendedAttributeType.UserApiToken);
        if (!tokensRequest.Succeeded)
        {
            tokensRequest.Messages.ForEach(x => Snackbar.Add($"Api Token retrieval failed: {x}", Severity.Error));
            return;
        }

        _userApiTokens = tokensRequest.Data.ToList();
        if (_userApiTokens.Any())
            _apiTokenGenButtonText = "Regenerate Token";
    }

    private async Task GenerateUserApiToken()
    {
        if (!_canGenerateApiTokens) return;

        var generateRequest = await AccountService.GenerateUserApiToken(CurrentUser.Id, _tokenTimeframe);
        if (!generateRequest.Succeeded)
        {
            generateRequest.Messages.ForEach(x => Snackbar.Add($"Failed to generate token: {x}", Severity.Error));
            return;
        }

        Snackbar.Add("Successfully generated API token!", Severity.Success);
        await GetUserApiTokens();
        StateHasChanged();
    }

    private async Task CopyToClipboard(string content)
    {
        var copyRequest = await WebClientService.InvokeClipboardCopy(content);
        if (!copyRequest.Succeeded)
        {
            copyRequest.Messages.ForEach(x => Snackbar.Add($"Failed to copy to clipboard: {x}", Severity.Error));
            return;
        }

        Snackbar.Add("Successfully copied your API token to your clipboard!", Severity.Info);
    }
    
    private IEnumerable<string> ValidatePasswordRequirements(string content)
    {
        var passwordIssues = AccountHelpers.GetAnyIssuesWithPassword(content);
        if (!string.IsNullOrEmpty(content) && passwordIssues.Any())
            yield return passwordIssues.First();
    }
    
    private IEnumerable<string> ValidatePasswordsMatch(string content)
    {
        if (!string.IsNullOrEmpty(content) &&
            !string.IsNullOrWhiteSpace(DesiredPassword) &&
            content != DesiredPassword)
            yield return "Desired & Confirm Passwords Don't Match";
    }
}