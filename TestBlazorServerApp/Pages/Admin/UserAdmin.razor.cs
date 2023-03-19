﻿using Application.Constants.Web;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Admin;

public partial class UserAdmin
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    private AppUserFull CurrentUser { get; set; } = new();
    private MudTable<AppUserDb> _table = new();
    private IEnumerable<AppUserDb> _pagedData = new List<AppUserDb>();
    private HashSet<AppUserDb> _selectedItems = new();
    private string _searchString = "";
    private int _totalUsers = 0;
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
            StateHasChanged();
        }
    }

    private async Task GetCurrentUser()
    {
        var userId = await CurrentUserService.GetCurrentUserId();
        if (userId is null)
            return;

        CurrentUser = (await UserRepository.GetByIdFullAsync((Guid) userId)).Result!;
    }
    
    private async Task<TableData<AppUserDb>> ServerReload(TableState state)
    {
        var usersResult = await UserRepository.SearchAsync(_searchString);
        if (!usersResult.Success)
        {
            Snackbar.Add(usersResult.ErrorMessage, Severity.Error);
            return new TableData<AppUserDb>();
        }

        var data = usersResult.Result!;
        
        data = data.Where(user =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (user.Username.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (user.Email!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (user.FirstName!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (user.LastName!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (user.Id.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            return false;
        }).ToArray();
        
        _totalUsers = data.Count();
        
        switch (state.SortLabel)
        {
            case "Id":
                data = data.OrderByDirection(state.SortDirection, o => o.Id);
                break;
            case "Username":
                data = data.OrderByDirection(state.SortDirection, o => o.Username);
                break;
            case "Email":
                data = data.OrderByDirection(state.SortDirection, o => o.Email);
                break;
            case "EmailConfirmed":
                data = data.OrderByDirection(state.SortDirection, o => o.EmailConfirmed);
                break;
            case "IsActive":
                data = data.OrderByDirection(state.SortDirection, o => o.IsActive);
                break;
        }

        _pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        
        return new TableData<AppUserDb>() {TotalItems = _totalUsers, Items = _pagedData};
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task EnableAccounts()
    {
        foreach (var account in _selectedItems)
        {
            var result = await AccountService.ChangeUserEnabledState(account.Id, true);
            if (!result.Succeeded)
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            else
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Success));
        }
    }

    private async Task DisableAccounts()
    {
        foreach (var account in _selectedItems)
        {
            var result = await AccountService.ChangeUserEnabledState(account.Id, false);
            if (!result.Succeeded)
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            else
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Success));
        }
        
    }

    private async Task ForcePasswordResets()
    {
        foreach (var account in _selectedItems)
        {
            var result = await AccountService.ForceUserPasswordReset(account.Id);
            if (!result.Succeeded)
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            else
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Success));
        }
    }

    private void ViewUser(Guid userId)
    {
        var viewUserUri = QueryHelpers.AddQueryString(AppRouteConstants.Admin.UserView, "userId", userId.ToString());
        NavManager.NavigateTo(viewUserUri, true);
    }
}