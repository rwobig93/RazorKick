using Application.Constants.Identity;
using Application.Helpers.Runtime;
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
    private UserBasicResponse _loggedInUser = new();
    private string _mfaKey = string.Empty;
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
        var user = await CurrentUserService.GetCurrentUserBasic();
        if (user is null)
            return;

        _loggedInUser = user;
    }

    private void RegisterTotp()
    {
        _mfaKey = MfaService.GenerateKeyString();
        _qrCodeSrc = QrCodeService.GenerateQrCodeSrc(_mfaKey);
    }

    private void CheckTotpCode()
    {
        _totpCorrect = MfaService.IsPasscodeCorrect(_totpCode, _mfaKey);
    }
}