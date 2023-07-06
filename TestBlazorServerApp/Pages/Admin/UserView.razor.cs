using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity;
using Application.Models.Identity.User;
using Application.Services.Identity;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Admin;

public partial class UserView
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    
    [Parameter] public Guid UserId { get; set; }

    private ClaimsPrincipal _currentUser = new();
    private AppUserFull _viewingUser = new();
    private string? _createdByUsername = "";
    private string? _modifiedByUsername = "";
    private DateTime? _createdOn;
    private DateTime? _modifiedOn;

    private bool _invalidDataProvided;
    private bool _editMode;
    private bool _canEditUsers;
    private bool _canEnableUsers;
    private bool _canDisableUsers;
    private bool _canViewRoles;
    private bool _canEditRoles;
    private bool _canViewPermissions;
    private bool _canAddPermissions;
    private bool _canRemovePermissions;
    private bool _enableEditable;
    private string _editButtonText = "Enable Edit Mode";
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                ParseParametersFromUri();
                await GetViewingUser();
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

        if (!queryParameters.TryGetValue("userId", out var queryUserId)) return;
        
        var providedIdIsValid = Guid.TryParse(queryUserId, out var parsedUserId);
        if (!providedIdIsValid)
            throw new InvalidDataException("Invalid UserId provided for user view");
            
        UserId = parsedUserId;
    }

    private async Task GetViewingUser()
    {
        _viewingUser = (await UserService.GetByIdFullAsync(UserId)).Data!;
        _createdByUsername = (await UserService.GetByIdAsync(_viewingUser.CreatedBy)).Data?.Username;
        // TODO: Add timezone id gather from local system/client
        _createdOn = _viewingUser.CreatedOn.ConvertToLocal(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        
        if (_viewingUser.LastModifiedBy is not null)
        {
            _modifiedByUsername = (await UserService.GetByIdAsync((Guid)_viewingUser.LastModifiedBy)).Data?.Username;
            _modifiedOn = ((DateTime) _viewingUser.LastModifiedOn!).ConvertToLocal(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }
    }

    private async Task GetPermissions()
    {
        _currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _canEditUsers = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Users.Edit);
        _canDisableUsers = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Users.Disable);
        _canEnableUsers = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Users.Enable);
        _canViewRoles = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Roles.View);
        _canEditRoles = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Roles.Edit);
        _canViewPermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.View);
        _canAddPermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.Add);
        _canRemovePermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.Remove);
    }

    private async Task Save()
    {
        var updateResult = await UserService.UpdateAsync(_viewingUser.ToUpdate());
        if (!updateResult.Succeeded)
        {
            updateResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }
        
        ToggleEditMode();
        await GetViewingUser();
        Snackbar.Add("User successfully updated!", Severity.Success);
        StateHasChanged();
    }

    private bool CanEditEnabled()
    {
        if (!_canDisableUsers && !_canEnableUsers) return false;
        if (_canDisableUsers && _canEnableUsers) return true;
        if (_canDisableUsers && _viewingUser.AuthState == AuthState.Enabled) return true;
        if (_canEnableUsers && _viewingUser.AuthState == AuthState.Disabled) return true;

        return false;
    }

    private void ToggleEditMode()
    {
        _editMode = !_editMode;

        _editButtonText = _editMode ? "Disable Edit Mode" : "Enable Edit Mode";
        _enableEditable = _editMode && CanEditEnabled();
    }

    private void GoBack()
    {
        NavManager.NavigateTo(AppRouteConstants.Admin.Users);
    }

    private async Task EditRoles()
    {
        var dialogParameters = new DialogParameters() {{"UserId", _viewingUser.Id}};
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };

        var dialog = DialogService.Show<UserRoleDialog>("Edit User Roles", dialogParameters, dialogOptions);
        var dialogResult = await dialog.Result;
        if (!dialogResult.Cancelled && (bool)dialogResult.Data)
        {
            await GetViewingUser();
            StateHasChanged();
        }
    }

    private async Task EditPermissions()
    {
        var dialogParameters = new DialogParameters() {{"UserId", _viewingUser.Id}};
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };

        var dialog = DialogService.Show<UserPermissionDialog>("Edit User Permissions", dialogParameters, dialogOptions);
        var dialogResult = await dialog.Result;
        if (!dialogResult.Cancelled && (bool)dialogResult.Data)
        {
            await GetViewingUser();
            StateHasChanged();
        }
    }
}