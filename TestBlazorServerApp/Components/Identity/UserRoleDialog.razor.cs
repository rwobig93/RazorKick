using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Helpers.Identity;
using Application.Helpers.Runtime;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Components;
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Components.Identity;

public partial class UserRoleDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    [Inject] private IAppRoleRepository RoleRepository { get; init; } = null!;

    [Parameter] public Guid UserId { get; set; }
    
    private List<AppRoleDb> _assignedRoles = new();
    private List<AppRoleDb> _availableRoles = new();
    private HashSet<AppRoleDb> _addRoles = new();
    private HashSet<AppRoleDb> _removeRoles = new();
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
        _canRemoveRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Remove);
        _canAddRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Add);
    }

    private async Task GetRoleLists()
    {
        if (!_canRemoveRoles && !_canAddRoles)
            return;

        var userRoles = await RoleRepository.GetRolesForUser(UserId);
        if (!userRoles.Success)
        {
            Snackbar.Add(userRoles.ErrorMessage, Severity.Error);
            return;
        }

        if (_canRemoveRoles)
        {
            _assignedRoles = userRoles.Result!.ToList();
        }

        if (_canAddRoles)
        {
            var allRoles = await RoleRepository.GetAllAsync();
            if (!allRoles.Success)
            {
                Snackbar.Add(allRoles.ErrorMessage, Severity.Error);
                return;
            }

            _availableRoles = allRoles.Result!.Except(userRoles.Result!).ToList();
        }
    }

    private void AddRoles()
    {
        foreach (var role in _addRoles)
        {
            _availableRoles.Remove(role);
            _assignedRoles.Add(role);
        }
    }

    private void RemoveRoles()
    {
        foreach (var role in _removeRoles)
        {
            _assignedRoles.Remove(role);
            _availableRoles.Add(role);
        }
    }
    
    private async Task Save()
    {
        var currentRoles = await RoleRepository.GetRolesForUser(UserId);
        if (!currentRoles.Success)
        {
            Snackbar.Add(currentRoles.ErrorMessage, Severity.Error);
            return;
        }

        foreach (var role in _assignedRoles.Where(role => currentRoles.Result!.All(x => x.Id != role.Id)))
        {
            var addRole = await RoleRepository.AddUserToRoleAsync(UserId, role.Id);
            if (!addRole.Success)
            {
                Snackbar.Add(addRole.ErrorMessage, Severity.Error);
                continue;
            }

            Snackbar.Add($"Successfully added role {role.Name}", Severity.Success);
        }

        foreach (var role in currentRoles.Result!.Where(role => _assignedRoles.All(x => x.Id != role.Id)))
        {
            var removeRole = await RoleRepository.RemoveUserFromRoleAsync(UserId, role.Id);
            if (!removeRole.Success)
            {
                Snackbar.Add(removeRole.ErrorMessage, Severity.Error);
                continue;
            }

            Snackbar.Add($"Successfully removed role {role.Name}", Severity.Success);
        }
        
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Close();
    }
}