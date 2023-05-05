using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Microsoft.AspNetCore.Components;
using Shared.Responses.Identity;

namespace TestBlazorServerApp.Pages;

public partial class Index
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IQrCodeService QrCodeService { get; init; } = null!;
    [Inject] private IMfaService MfaService { get; init; } = null!;
    
    private AppUserFull _loggedInUser = new();
    private string _mfaKey = "UCCKF4EXQJC52KEOPGOCOCDMNAH7KUOO";
    private string _qrCodeSrc = string.Empty;
    private string _totpCode = string.Empty;
    private bool _totpCorrect;
    
    private bool _canViewApi;
    private bool _canViewJobs;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await UpdateLoggedInUser();
                await GetPermissions();
                StateHasChanged();
            }
        }
        catch
        {
            // User has old saved token so we'll force a local storage clear and de-authenticate then redirect due to being unauthorized
            await AccountService.LogoutGuiAsync(Guid.Empty);
            StateHasChanged();
        }
    }

    private async Task GetPermissions()
    {
        var currentUser = await CurrentUserService.GetCurrentUserPrincipal();
        _canViewApi = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Api.View);
        _canViewJobs = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Jobs.View);
    }

    private async Task UpdateLoggedInUser()
    {
        var user = await CurrentUserService.GetCurrentUserFull();
        if (user is null)
            return;

        _loggedInUser = user;
    }

    private void RegisterTotp()
    {
        // TOTP Syntax: otpauth://totp/Name:email@example.com?secret=<code>&&issuer=Name&algorithm=SHA512&digits=6&period=30
        // _mfaKey = MfaService.GenerateKeyString();
        var appName = ServerState.ApplicationName; // UrlHelpers.SanitizeTextForUrl(ServerState.ApplicationName);
        var qrCodeContent =
            $"otpauth://totp/{appName}?secret={_mfaKey}&issuer={appName}&algorithm=SHA1&digits=6&period=30";
        _qrCodeSrc = QrCodeService.GenerateQrCodeSrc(qrCodeContent);
    }

    private void CheckTotpCode()
    {
        _totpCorrect = MfaService.IsPasscodeCorrect(_totpCode, _mfaKey, out var matchedCount);
        Snackbar.Add($"Matched Count: {matchedCount}", Severity.Info);
    }
}