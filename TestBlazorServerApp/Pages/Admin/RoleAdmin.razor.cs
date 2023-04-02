using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace TestBlazorServerApp.Pages.Admin;

public partial class RoleAdmin
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppRoleRepository RoleRepository { get; init; } = null!;
    private MudTable<AppRoleDb> _table = new();
    private IEnumerable<AppRoleDb> _pagedData = new List<AppRoleDb>();
    private string _searchString = "";
    private int _totalRoles = 0;
    
    private async Task<TableData<AppRoleDb>> ServerReload(TableState state)
    {
        var rolesResult = await RoleRepository.SearchAsync(_searchString);
        if (!rolesResult.Success)
        {
            Snackbar.Add(rolesResult.ErrorMessage, Severity.Error);
            return new TableData<AppRoleDb>();
        }

        var data = rolesResult.Result!;
        
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
        
        return new TableData<AppRoleDb>() {TotalItems = _totalRoles, Items = _pagedData};
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