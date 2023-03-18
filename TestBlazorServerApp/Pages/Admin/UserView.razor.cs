using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace TestBlazorServerApp.Pages.Admin;

public partial class UserView
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    
    [Parameter] public Guid UserId { get; set; }

    private ClaimsPrincipal _currentUser = new();
    private AppUserFull _viewingUser = new();
    private string? _createdByUsername = "";
    private string? _modifiedByUsername = "";

    private bool _invalidDataProvided;
    private bool _editMode;
    private bool _canEditUsers;
    private bool _canViewRoles;
    private bool _canEditRoles;
    private bool _canViewPermissions;
    private bool _canAddPermissions;
    private bool _canRemovePermissions;
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

    private bool ParseParametersFromUri()
    {
        var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);
        
        if (queryParameters.TryGetValue("userId", out var queryUserId))
        {
            var providedIdIsValid = Guid.TryParse(queryUserId, out var parsedUserId);
            if (!providedIdIsValid)
                throw new InvalidDataException("Invalid UserId provided for user view");
            
            UserId = parsedUserId;
        }

        return true;
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
        _canAddPermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.Add);
        _canRemovePermissions = await AuthorizationService.UserHasPermission(_currentUser, PermissionConstants.Permissions.Remove);
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

    private void GoBack()
    {
        NavManager.NavigateTo(AppRouteConstants.Admin.Users);
    }
}