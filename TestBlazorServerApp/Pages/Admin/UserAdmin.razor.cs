using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Models.Identity;
using Application.Models.Identity.User;
using Application.Services.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace TestBlazorServerApp.Pages.Admin;

public partial class UserAdmin
{
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    private MudTable<AppUserSlim> _table = new();
    private IEnumerable<AppUserSlim> _pagedData = new List<AppUserSlim>();
    private HashSet<AppUserSlim> _selectedItems = new();
    private string _searchString = "";
    private int _totalUsers;

    private bool _canEnableUsers;
    private bool _canDisableUsers;
    private bool _canResetPasswords;
    private bool _allowUserSelection;
    
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
        _canResetPasswords = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Users.ResetPassword);
        _canDisableUsers = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Users.Disable);
        _canEnableUsers = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Users.Enable);

        _allowUserSelection = _canResetPasswords || _canDisableUsers || _canEnableUsers;
    }
    
    private async Task<TableData<AppUserSlim>> ServerReload(TableState state)
    {
        var usersResult = await UserService.SearchAsync(_searchString);
        if (!usersResult.Succeeded)
        {
            usersResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return new TableData<AppUserSlim>();
        }

        var data = usersResult.Data;
        
        data = data.Where(user =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (user.Username.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (user.EmailAddress!.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
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
                data = data.OrderByDirection(state.SortDirection, o => o.EmailAddress);
                break;
            case "IsActive":
                data = data.OrderByDirection(state.SortDirection, o => o.AuthState);
                break;
        }

        _pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        
        return new TableData<AppUserSlim>() {TotalItems = _totalUsers, Items = _pagedData};
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task EnableAccounts()
    {
        if (!_canEnableUsers)
        {
            Snackbar.Add("You don't have permission to enable accounts, how'd you initiate this request!?", Severity.Error);
            return;
        }

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
        if (!_canDisableUsers)
        {
            Snackbar.Add("You don't have permission to disable accounts, how'd you initiate this request!?", Severity.Error);
            return;
        }

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
        if (!_canResetPasswords)
        {
            Snackbar.Add("You don't have permission to reset passwords, how'd you initiate this request!?", Severity.Error);
            return;
        }

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
        // TODO: Viewing the Anonymous User goes to a page not found, either need to troubleshoot or handle this one-off
        var viewUserUri = QueryHelpers.AddQueryString(AppRouteConstants.Admin.UserView, "userId", userId.ToString());
        NavManager.NavigateTo(viewUserUri);
    }
}