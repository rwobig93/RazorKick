using Application.Helpers.Runtime;
using Application.Models.Identity.Role;

namespace RazorKick.Components.Identity;

public partial class RoleCreateDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; init; } = null!;

    [Inject] private IAppRoleService RoleService { get; init; } = null!;

    private Guid _currentUserId;
    private AppRoleCreate _newRole = new();
    
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
        _currentUserId = (await CurrentUserService.GetCurrentUserId()).GetFromNullable();
    }
    
    private async Task Save()
    {
        var createRoleRequest = await RoleService.CreateAsync(_newRole, _currentUserId);
        if (!createRoleRequest.Succeeded)
        {
            createRoleRequest.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }
        
        Snackbar.Add("Successfully created role!", Severity.Success);
        MudDialog.Close(DialogResult.Ok(createRoleRequest.Data));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}