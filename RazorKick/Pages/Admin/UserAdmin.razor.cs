using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Models.Identity.User;
using Domain.Enums.Identity;
using RazorKick.Components.Identity;

namespace RazorKick.Pages.Admin;

public partial class UserAdmin
{
    [CascadingParameter] public MainLayout ParentLayout { get; set; } = null!;

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
    private bool _canAdminServiceAccounts;

    private bool _dense = true;
    private bool _hover = true;
    private bool _striped = true;
    private bool _bordered;
    private bool _filterLockedOut;
    private bool _filterDisabled;

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
        _canAdminServiceAccounts = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.ServiceAccounts.Admin);

        _allowUserSelection = _canResetPasswords || _canDisableUsers || _canEnableUsers;
    }
    
    private async Task<TableData<AppUserSlim>> ServerReload(TableState state)
    {
        var usersResult = await UserService.SearchPaginatedAsync(_searchString, state.Page, state.PageSize);
        if (!usersResult.Succeeded)
        {
            usersResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return new TableData<AppUserSlim>();
        }

        _pagedData = usersResult.Data.ToArray();
        _totalUsers = (await UserService.GetCountAsync()).Data;

        if (_filterDisabled)
            _pagedData = _pagedData.Where(x => x.AuthState == AuthState.Disabled);
        if (_filterLockedOut)
            _pagedData = _pagedData.Where(x => x.AuthState == AuthState.LockedOut);

        _pagedData = state.SortLabel switch
        {
            "Id" => _pagedData.OrderByDirection(state.SortDirection, o => o.Id),
            "Username" => _pagedData.OrderByDirection(state.SortDirection, o => o.Username),
            "Enabled" => _pagedData.OrderByDirection(state.SortDirection, o => o.AuthState),
            "AccountType" => _pagedData.OrderByDirection(state.SortDirection, o => o.AccountType),
            "Notes" => _pagedData.OrderByDirection(state.SortDirection, o => o.Notes),
            _ => _pagedData
        };
        
        return new TableData<AppUserSlim>() {TotalItems = _totalUsers, Items = _pagedData};
    }

    private async Task SearchText(string text)
    {
        _searchString = text;
        await _table.ReloadServerData();
        _selectedItems = new HashSet<AppUserSlim>();
        StateHasChanged();
    }

    private async Task ReloadSearch()
    {
        await SearchText(_searchString);
    }

    private async Task ClearSearch()
    {
        _filterDisabled = false;
        _filterLockedOut = false;
        await SearchText("");
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
            var result = await AccountService.SetAuthState(account.Id, AuthState.Enabled);
            if (!result.Succeeded)
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            else
            {
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Success));
                await SearchText(_searchString);
            }
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
            var result = await AccountService.SetAuthState(account.Id, AuthState.Disabled);
            if (!result.Succeeded)
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            else
            {
                result.Messages.ForEach(x => Snackbar.Add(x, Severity.Success));
                await SearchText(_searchString);
            }
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
            if (account.AccountType != AccountType.User)
            {
                Snackbar.Add($"Account {account.Username} is a service account, can't force a password reset {account.AccountType} accounts",
                    Severity.Error);
                continue;
            }
            
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
        NavManager.NavigateTo(viewUserUri);
    }

    private async Task CreateServiceAccount()
    {
        if (!_canAdminServiceAccounts)
        {
            Snackbar.Add("You don't have permission to create accounts, how'd you initiate this request!?", Severity.Error);
            return;
        }
        
        var createParameters = new DialogParameters() { {"ServiceAccountId", Guid.Empty} };
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };
        var createAccountDialog = await DialogService.Show<ServiceAccountAdminDialog>(
            "Create Service Account", createParameters, dialogOptions).Result;
        if (createAccountDialog.Canceled)
            return;

        var createdPassword = (string?) createAccountDialog.Data;
        if (string.IsNullOrWhiteSpace(createdPassword))
        {
            Snackbar.Add("Something happened and we couldn't retrieve the password for this account, please contact the administrator",
                Severity.Error);
            await ReloadSearch();
            return;
        }
        
        var copyParameters = new DialogParameters()
        {
            {"Title", "Please copy the account password and save it somewhere safe"},
            {"FieldLabel", "Service Account Password"},
            {"TextToDisplay", new string('*', createdPassword.Length)},
            {"TextToCopy", createdPassword}
        };
        await DialogService.Show<CopyTextDialog>("Service Account Password", copyParameters, dialogOptions).Result;
        await ReloadSearch();
    }
}