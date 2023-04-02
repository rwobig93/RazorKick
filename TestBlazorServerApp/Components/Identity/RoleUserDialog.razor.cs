using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Components.Identity;

public partial class RoleUserDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    [Inject] private IAppRoleRepository RoleRepository { get; init; } = null!;

    [Parameter] public Guid RoleId { get; set; }

    private List<AppUserDb> _assignedUsers = new();
    private List<AppUserDb> _availableUsers = new();
    private HashSet<AppUserDb> _addUsers = new();
    private HashSet<AppUserDb> _removeUsers = new();
    private Guid _currentUserId;
    private bool _canRemoveRoles;
    private bool _canAddRoles;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetPermissions();
            await GetRoleLists();
            StateHasChanged();
        }
    }

    private async Task GetPermissions()
    {
        var currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _currentUserId = CurrentUserService.GetIdFromPrincipal(currentUser);
        _canRemoveRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Remove);
        _canAddRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Add);
    }

    private async Task GetRoleLists()
    {
        if (!_canRemoveRoles && !_canAddRoles)
            return;

        var roleUsers = await RoleRepository.GetUsersForRole(RoleId);
        if (!roleUsers.Success)
        {
            Snackbar.Add(roleUsers.ErrorMessage, Severity.Error);
            return;
        }

        if (_canRemoveRoles)
        {
            _assignedUsers = roleUsers.Result!.ToList();
        }

        if (_canAddRoles)
        {
            var allUsers = await UserRepository.GetAllAsync();
            if (!allUsers.Success)
            {
                Snackbar.Add(allUsers.ErrorMessage, Severity.Error);
                return;
            }

            _availableUsers = allUsers.Result!.Where(x => roleUsers.Result!.All(r => r.Id != x.Id)).ToList();
        }
    }

    private void AddUsers()
    {
        foreach (var user in _addUsers)
        {
            _availableUsers.Remove(user);
            _assignedUsers.Add(user);
        }
    }

    private void RemoveUsers()
    {
        foreach (var user in _removeUsers)
        {
            _assignedUsers.Remove(user);
            _availableUsers.Add(user);
        }
    }
    
    private async Task Save()
    {
        var currentUsers = await RoleRepository.GetUsersForRole(RoleId);
        if (!currentUsers.Success)
        {
            Snackbar.Add(currentUsers.ErrorMessage, Severity.Error);
            return;
        }

        foreach (var user in _assignedUsers.Where(role => currentUsers.Result!.All(x => x.Id != role.Id)))
        {
            var addRole = await RoleRepository.AddUserToRoleAsync(user.Id, RoleId, _currentUserId);
            if (!addRole.Success)
            {
                Snackbar.Add(addRole.ErrorMessage, Severity.Error);
                continue;
            }

            Snackbar.Add($"Successfully added user {user.Username}", Severity.Success);
        }

        foreach (var user in currentUsers.Result!.Where(user => _assignedUsers.All(x => x.Id != user.Id)))
        {
            var removeRole = await RoleRepository.RemoveUserFromRoleAsync(user.Id, RoleId, _currentUserId);
            if (!removeRole.Success)
            {
                Snackbar.Add(removeRole.ErrorMessage, Severity.Error);
                continue;
            }

            Snackbar.Add($"Successfully removed user {user.Username}", Severity.Success);
        }
        
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}