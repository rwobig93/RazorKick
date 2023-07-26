using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Models.Identity.Role;
using RazorKick.Components.Identity;

namespace RazorKick.Pages.Admin;

public partial class RoleAdmin
{
    [CascadingParameter] public MainLayout ParentLayout { get; set; } = null!;

    [Inject] private IAppRoleService RoleService { get; init; } = null!;
    
    private MudTable<AppRoleSlim> _table = new();
    private IEnumerable<AppRoleSlim> _pagedData = new List<AppRoleSlim>();
    private string _searchString = "";
    private int _totalRoles;
    private bool _dense = true;
    private bool _hover = true;
    private bool _striped = true;
    private bool _bordered;

    private bool _canCreateRoles;

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
        _canCreateRoles = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Roles.Create);
    }
    
    private async Task<TableData<AppRoleSlim>> ServerReload(TableState state)
    {
        var rolesResult = await RoleService.SearchPaginatedAsync(_searchString, state.Page, state.PageSize);
        if (!rolesResult.Succeeded)
        {
            rolesResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return new TableData<AppRoleSlim>();
        }

        _pagedData = rolesResult.Data.ToArray();
        _totalRoles = (await RoleService.GetCountAsync()).Data;

        _pagedData = state.SortLabel switch
        {
            "Id" => _pagedData.OrderByDirection(state.SortDirection, o => o.Id),
            "Name" => _pagedData.OrderByDirection(state.SortDirection, o => o.Name),
            _ => _pagedData
        };
        
        return new TableData<AppRoleSlim>() {TotalItems = _totalRoles, Items = _pagedData};
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private void ViewRole(Guid roleId)
    {
        var viewRoleUri = QueryHelpers.AddQueryString(AppRouteConstants.Admin.RoleView, "roleId", roleId.ToString());
        NavManager.NavigateTo(viewRoleUri);
    }

    private async Task CreateRole()
    {
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };
        var createRoleDialog = await DialogService.Show<RoleCreateDialog>("Create New Role", dialogOptions).Result;
        if (createRoleDialog.Canceled)
            return;

        var createdRoleId = (Guid) createRoleDialog.Data;
        var newRoleViewUrl = QueryHelpers.AddQueryString(AppRouteConstants.Admin.RoleView, "roleId", createdRoleId.ToString());
        NavManager.NavigateTo(newRoleViewUrl);
    }
}