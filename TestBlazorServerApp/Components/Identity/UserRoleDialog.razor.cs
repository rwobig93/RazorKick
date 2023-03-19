using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Helpers.Identity;
using Application.Helpers.Runtime;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Microsoft.AspNetCore.Components;
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Components.Identity;

public partial class UserRoleDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;

    private bool _canRemoveRoles;
    private bool _canAddRoles;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetPermissions();
            StateHasChanged();
        }
    }

    private async Task GetPermissions()
    {
        var currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _canRemoveRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Remove);
        _canAddRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Add);
    }
    
    private async Task Save()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Close();
    }
}