using Application.Responses.Identity;
using Application.Services.Identity;
using Microsoft.AspNetCore.Components;

namespace RazorKick.Components.Account;

public partial class AccountSettingsNavBar
{
    [Inject] private IAppAccountService AccountService { get; set; } = null!;
    private UserBasicResponse CurrentUser { get; set; } = new();
    
    
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
        CurrentUser = await CurrentUserService.GetCurrentUserBasic() ?? new UserBasicResponse();
    }

    private void NavigateToPage(string route)
    {
        NavManager.NavigateTo(route);
    }
}