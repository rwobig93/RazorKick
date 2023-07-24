using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Models.Identity.User;
using Domain.Enums.Identity;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Admin;

public partial class ServiceAccountAdmin
{
    [CascadingParameter] public MainLayout ParentLayout { get; set; } = null!;

    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IAppUserService UserService { get; init; } = null!;
    private MudTable<AppUserSlim> _table = new();
    private IEnumerable<AppUserSlim> _pagedData = new List<AppUserSlim>();
    private HashSet<AppUserSlim> _selectedItems = new();
    private string _searchString = "";
    private int _totalUsers;

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
        _canAdminServiceAccounts = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.ServiceAccounts.Admin);
    }
    
    private async Task<TableData<AppUserSlim>> ServerReload(TableState state)
    {
        var usersResult = await UserService.GetAllPaginatedAsync(state.Page, state.PageSize);
        if (!usersResult.Succeeded)
        {
            usersResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return new TableData<AppUserSlim>();
        }

        _pagedData = usersResult.Data.Where(account =>
        {
            if (account.Username.Contains(_searchString))
                return true;
            if (account.FirstName?.Contains(_searchString) ?? "".Contains(_searchString))
                return true;
            if (account.LastName?.Contains(_searchString) ?? "".Contains(_searchString))
                return true;
            if (account.Notes?.Contains(_searchString) ?? "".Contains(_searchString))
                return true;

            return false;
        }).ToArray();

        if (_filterDisabled)
            _pagedData = _pagedData.Where(x => x.AuthState == AuthState.Disabled);
        if (_filterLockedOut)
            _pagedData = _pagedData.Where(x => x.AuthState == AuthState.LockedOut);

        _pagedData = _pagedData.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        _totalUsers = usersResult.Data.Count();

        _pagedData = state.SortLabel switch
        {
            "Id" => _pagedData.OrderByDirection(state.SortDirection, o => o.Id),
            "Username" => _pagedData.OrderByDirection(state.SortDirection, o => o.Username),
            "FirstName" => _pagedData.OrderByDirection(state.SortDirection, o => o.FirstName),
            "LastName" => _pagedData.OrderByDirection(state.SortDirection, o => o.LastName),
            "Enabled" => _pagedData.OrderByDirection(state.SortDirection, o => o.AuthState),
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
        if (!_canAdminServiceAccounts)
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
        StateHasChanged();
    }

    private async Task DisableAccounts()
    {
        if (!_canAdminServiceAccounts)
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
        StateHasChanged();
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

    private async Task EditServiceAccount()
    {
        if (!_canAdminServiceAccounts)
        {
            Snackbar.Add("You don't have permission to edit accounts, how'd you initiate this request!?", Severity.Error);
            return;
        }
        
        var updateParameters = new DialogParameters() { {"ServiceAccountId", _selectedItems.First().Id} };
        var dialogOptions = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, CloseOnEscapeKey = true };
        var updateAccountDialog = await DialogService.Show<ServiceAccountAdminDialog>(
            "Update Service Account", updateParameters, dialogOptions).Result;
        if (updateAccountDialog.Canceled)
            return;

        var createdPassword = (string?) updateAccountDialog.Data;
        if (string.IsNullOrWhiteSpace(createdPassword))
        {
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