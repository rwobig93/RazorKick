using Application.Constants.Web;
using Application.Models.Identity.Role;
using Application.Services.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace TestBlazorServerApp.Pages.Admin;

public partial class RoleAdmin
{
    [Inject] private IAppRoleService RoleService { get; init; } = null!;
    private MudTable<AppRoleSlim> _table = new();
    private IEnumerable<AppRoleSlim> _pagedData = new List<AppRoleSlim>();
    private string _searchString = "";
    private int _totalRoles;
    
    private async Task<TableData<AppRoleSlim>> ServerReload(TableState state)
    {
        var rolesResult = await RoleService.SearchAsync(_searchString);
        if (!rolesResult.Succeeded)
        {
            rolesResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return new TableData<AppRoleSlim>();
        }

        var data = rolesResult.Data;
        
        data = data.Where(user =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (user.Name!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (user.Description!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            return false;
        }).ToArray();
        
        _totalRoles = data.Count();

        data = state.SortLabel switch
        {
            "Id" => data.OrderByDirection(state.SortDirection, o => o.Id),
            "Name" => data.OrderByDirection(state.SortDirection, o => o.Name),
            _ => data
        };

        _pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        
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
}