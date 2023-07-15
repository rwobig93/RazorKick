﻿using Application.Constants.Web;
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
}