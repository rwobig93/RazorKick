using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Helpers.Identity;
using Application.Helpers.Runtime;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Components;
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Components.Identity;

public partial class UserPermissionDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    [Inject] private IAppPermissionRepository PermissionRepository { get; init; } = null!;

    [Parameter] public Guid UserId { get; set; }
    
    private List<AppPermissionDb> _assignedPermissions = new();
    private List<AppPermissionDb> _availablePermissions = new();
    private HashSet<AppPermissionDb> _addPermissions = new();
    private HashSet<AppPermissionDb> _removePermissions = new();
    private Guid _currentUserId;
    private bool _canRemovePermissions;
    private bool _canAddPermissions;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUserPermissions();
            await GetPermissionLists();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUserPermissions()
    {
        var currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _currentUserId = CurrentUserService.GetIdFromPrincipal(currentUser);
        _canRemovePermissions = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Permissions.Remove);
        _canAddPermissions = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Permissions.Add);
    }

    private async Task GetPermissionLists()
    {
        if (!_canRemovePermissions && !_canAddPermissions)
            return;

        var userPermissions = await PermissionRepository.GetAllDirectForUserAsync(UserId);
        if (!userPermissions.Success)
        {
            Snackbar.Add(userPermissions.ErrorMessage, Severity.Error);
            return;
        }

        if (_canRemovePermissions)
        {
            _assignedPermissions = userPermissions.Result!.ToList();
        }

        if (_canAddPermissions)
        {
            var allPermissions = await PermissionRepository.GetAllAsync();
            if (!allPermissions.Success)
            {
                Snackbar.Add(allPermissions.ErrorMessage, Severity.Error);
                return;
            }

            _availablePermissions = allPermissions.Result!.Where(x => userPermissions.Result!.All(r => r.Id != x.Id)).ToList();
        }
    }

    private void AddPermissions()
    {
        foreach (var permission in _addPermissions)
        {
            _availablePermissions.Remove(permission);
            _assignedPermissions.Add(permission);
        }
    }

    private void RemovePermissions()
    {
        foreach (var permission in _removePermissions)
        {
            _assignedPermissions.Remove(permission);
            _availablePermissions.Add(permission);
        }
    }
    
    private async Task Save()
    {
        var currentPermissions = await PermissionRepository.GetAllDirectForUserAsync(UserId);
        if (!currentPermissions.Success)
        {
            Snackbar.Add(currentPermissions.ErrorMessage, Severity.Error);
            return;
        }

        foreach (var permission in _assignedPermissions.Where(perm => currentPermissions.Result!.All(x => x.Id != perm.Id)))
        {
            var addPermission = await PermissionRepository.CreateAsync(new AppPermissionCreate
            {
                UserId = UserId,
                ClaimType = permission.ClaimType!,
                ClaimValue = permission.ClaimValue!,
                Name = permission.Name,
                Group = permission.Group,
                Access = permission.Access,
                Description = permission.Description,
                CreatedBy = _currentUserId
            }, _currentUserId);
            if (!addPermission.Success)
            {
                Snackbar.Add(addPermission.ErrorMessage, Severity.Error);
                continue;
            }

            Snackbar.Add($"Successfully added permission {permission.Group}.{permission.Name}.{permission.Access}", Severity.Success);
        }

        foreach (var permission in currentPermissions.Result!.Where(perm => _assignedPermissions.All(x => x.Id != perm.Id)))
        {
            var removePermission = await PermissionRepository.DeleteAsync(permission.Id, _currentUserId);
            if (!removePermission.Success)
            {
                Snackbar.Add(removePermission.ErrorMessage, Severity.Error);
                continue;
            }

            Snackbar.Add($"Successfully removed permission {permission.Group}.{permission.Name}.{permission.Access}", Severity.Success);
        }
        
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}