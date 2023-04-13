using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Admin;

public partial class RoleView
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppRoleRepository RoleRepository { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    
    [Parameter] public Guid RoleId { get; set; }

    private ClaimsPrincipal _currentUser = new();
    private AppRoleFull _viewingRole = new();
    private string? _createdByUsername = "";
    private string? _modifiedByUsername = "";
    private const string DateDisplayFormat = "MM/dd/yyyy hh:mm:ss tt zzz";

    private bool _invalidDataProvided;
    private bool _editMode;
    private bool _canEditRoles;
    private bool _canAddRoles;
    private bool _canRemoveRoles;
    private bool _canViewPermissions;
    private bool _canAddPermissions;
    private bool _canRemovePermissions;
    private bool _canViewUsers;
    private string _editButtonText = "Enable Edit Mode";
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                ParseParametersFromUri();
                await GetViewingRole();
                await GetPermissions();
                StateHasChanged();
            }
        }
        catch
        {
            _invalidDataProvided = true;
            StateHasChanged();
        }
    }

    private void ParseParametersFromUri()
    {
        var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);

        if (!queryParameters.TryGetValue("roleId", out var queryRoleId)) return;
        
        var providedIdIsValid = Guid.TryParse(queryRoleId, out var parsedRoleId);
        if (!providedIdIsValid)
            throw new InvalidDataException("Invalid RoleId provided for role view");
            
        RoleId = parsedRoleId;
    }

    private async Task GetViewingRole()
    {
        _viewingRole = (await RoleRepository.GetByIdFullAsync(RoleId)).Result!;
        _createdByUsername = (await UserRepository.GetByIdAsync(_viewingRole.CreatedBy)).Result?.Username;
        if (_viewingRole.LastModifiedBy is null)
        {
            _modifiedByUsername = "";
            return;
        }
        
        _modifiedByUsername = (await UserRepository.GetByIdAsync((Guid)_viewingRole.LastModifiedBy)).Result?.Username;
    }

    private async Task GetPermissions()
    {
        _currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _canAddRoles = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Roles.Add);
        _canRemoveRoles = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Roles.Remove);
        _canEditRoles = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Roles.Edit);
        _canViewPermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.View);
        _canAddPermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.Add);
        _canRemovePermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.Remove);
        _canViewUsers = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Users.View);
    }

    private async Task Save()
    {
        var submittingUserId = CurrentUserService.GetIdFromPrincipal(_currentUser);
        
        var updateResult = await RoleRepository.UpdateAsync(_viewingRole.ToUpdate(), submittingUserId);
        if (!updateResult.Success)
        {
            Snackbar.Add(updateResult.ErrorMessage, Severity.Error);
            return;
        }
        
        ToggleEditMode();
        await GetViewingRole();
        Snackbar.Add("Role successfully updated!", Severity.Success);
        StateHasChanged();
    }

    private void ToggleEditMode()
    {
        _editMode = !_editMode;

        _editButtonText = _editMode ? "Disable Edit Mode" : "Enable Edit Mode";
    }

    private void GoBack()
    {
        NavManager.NavigateTo(AppRouteConstants.Admin.Roles);
    }

    private async Task EditUserMembership()
    {
        var dialogParameters = new DialogParameters() {{"RoleId", _viewingRole.Id}};
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };

        var dialog = DialogService.Show<RoleUserDialog>("Edit Role Membership", dialogParameters, dialogOptions);
        var dialogResult = await dialog.Result;
        if (!dialogResult.Cancelled && (bool)dialogResult.Data)
        {
            await GetViewingRole();
            StateHasChanged();
        }
    }

    private async Task EditPermissions()
    {
        var dialogParameters = new DialogParameters() {{"RoleId", _viewingRole.Id}};
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };

        var dialog = DialogService.Show<RolePermissionDialog>("Edit Role Permissions", dialogParameters, dialogOptions);
        var dialogResult = await dialog.Result;
        if (!dialogResult.Cancelled && (bool)dialogResult.Data)
        {
            await GetViewingRole();
            StateHasChanged();
        }
    }
}