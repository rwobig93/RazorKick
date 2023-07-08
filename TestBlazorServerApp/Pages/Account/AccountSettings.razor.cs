using Application.Mappers.Identity;
using Application.Models.Identity.User;
using Application.Services.Identity;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Pages.Account;

public partial class AccountSettings
{
    [Inject] private IAppUserService UserService { get; init; } = null!;
    private AppUserFull CurrentUser { get; set; } = new();
    
    
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

        CurrentUser = (await UserService.GetByIdFullAsync((Guid) userId)).Data!;
    }

    private async Task UpdateAccount()
    {
        var updatedAccount = CurrentUser.ToUpdate();
        var requestResult = await UserService.UpdateAsync(updatedAccount);
        if (!requestResult.Succeeded)
        {
            requestResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return;
        }

        Snackbar.Add("Account successfully updated!");
        StateHasChanged();
    }
}