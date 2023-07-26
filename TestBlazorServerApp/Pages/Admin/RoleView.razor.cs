using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Mappers.Identity;
using Application.Models.Identity.Role;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Admin;

public partial class RoleView
{
    [CascadingParameter] public MainLayout ParentLayout { get; set; } = null!;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    
    [Inject] private IAppRoleService RoleService { get; init; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    [Inject] private IWebClientService WebClientService { get; init; } = null!;
    
    [Parameter] public Guid RoleId { get; set; }

    private Guid _currentUserId;
    private AppRoleFull _viewingRole = new();
    private string? _createdByUsername = "";
    private string? _modifiedByUsername = "";
    private DateTime? _createdOn;
    private DateTime? _modifiedOn;
    private const string DateDisplayFormat = "MM/dd/yyyy hh:mm:ss tt zzz";
    private TimeZoneInfo _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT");

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
                await GetClientTimezone();
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
        _viewingRole = (await RoleService.GetByIdFullAsync(RoleId)).Data!;
        _createdByUsername = (await UserService.GetByIdAsync(_viewingRole.CreatedBy)).Data?.Username;
        _createdOn = _viewingRole.CreatedOn.ConvertToLocal(_localTimeZone);
        
        if (_viewingRole.LastModifiedBy is not null)
        {
            _modifiedByUsername = (await UserService.GetByIdAsync((Guid)_viewingRole.LastModifiedBy)).Data?.Username;
            if (_viewingRole.LastModifiedOn is not null)
                _modifiedOn = ((DateTime) _viewingRole.LastModifiedOn).ConvertToLocal(_localTimeZone);
        }
    }

    private async Task GetPermissions()
    {
        var currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _currentUserId = CurrentUserService.GetIdFromPrincipal(currentUser);
        _canAddRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Add);
        _canRemoveRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Remove);
        _canEditRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Edit);
        _canViewPermissions = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Permissions.View);
        _canAddPermissions = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Permissions.Add);
        _canRemovePermissions = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Permissions.Remove);
        _canViewUsers = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Users.View);
    }

    private async Task Save()
    {
        var updateResult = await RoleService.UpdateAsync(_viewingRole.ToUpdate(), _currentUserId);
        if (!updateResult.Succeeded)
        {
            updateResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
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

        var dialog = await DialogService.Show<RoleUserDialog>("Edit Role Membership", dialogParameters, dialogOptions).Result;
        if (!dialog.Canceled)
        {
            await GetViewingRole();
            StateHasChanged();
        }
    }

    private async Task EditPermissions()
    {
        var dialogParameters = new DialogParameters() {{"RoleId", _viewingRole.Id}};
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };

        var dialog = await DialogService.Show<RolePermissionDialog>("Edit Role Permissions", dialogParameters, dialogOptions).Result;
        if (!dialog.Canceled)
        {
            await GetViewingRole();
            StateHasChanged();
        }
    }

    private async Task GetClientTimezone()
    {
        var clientTimezoneRequest = await WebClientService.GetClientTimezone();
        if (!clientTimezoneRequest.Succeeded)
            clientTimezoneRequest.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));

        _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clientTimezoneRequest.Data);
    }
}