using System.Security.Claims;
using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Helpers.Identity;
using Application.Helpers.Runtime;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Components.Identity;

public partial class UserViewDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    
    [Parameter] public Guid UserId { get; set; }

    private ClaimsPrincipal _currentUser = new();
    private AppUserFull _viewingUser = new();
    private string? _createdByUsername = "";
    private string? _modifiedByUsername = "";
    
    private bool _editMode;
    private bool _canEditUsers;
    private bool _canViewRoles;
    private bool _canEditRoles;
    private bool _canViewPermissions;
    private bool _canEditPermissions;
    private string _editButtonText = "Enable Edit Mode";
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetViewingUser();
            await GetPermissions();
            StateHasChanged();
        }
    }

    private async Task GetViewingUser()
    {
        _viewingUser = (await UserRepository.GetByIdFullAsync(UserId)).Result!;
        _createdByUsername = (await UserRepository.GetByIdAsync(_viewingUser.CreatedBy)).Result?.Username;
        if (_viewingUser.LastModifiedBy is null)
        {
            _modifiedByUsername = "";
            return;
        }
        
        _modifiedByUsername = (await UserRepository.GetByIdAsync((Guid)_viewingUser.LastModifiedBy)).Result?.Username;
        // public List<AppRoleDb> Roles { get; set; } = new();
        // public List<AppUserExtendedAttributeDb> ExtendedAttributes { get; set; } = new();
        // public List<AppPermissionDb> Permissions { get; set; } = new();
    }

    private async Task GetPermissions()
    {
        _currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _canEditUsers = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Users.Edit);
        _canViewRoles = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Roles.View);
        _canEditRoles = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Roles.Edit);
        _canViewPermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.View);
        _canEditPermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.Edit);
    }

    private async Task Save()
    {
        await Task.CompletedTask;
        Snackbar.Add("User successfully updated!", Severity.Success);
    }

    private void ToggleEditMode()
    {
        _editMode = !_editMode;
        _editButtonText = _editMode ? "Disable Edit Mode" : "Enable Edit Mode";
    }

    private void Close()
    {
        MudDialog.Close();
    }
}