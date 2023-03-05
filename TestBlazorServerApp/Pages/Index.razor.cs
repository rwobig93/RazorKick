using System.Reflection;
using Application.Constants.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Pages;

public partial class Index
{
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;

    private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name ?? "TestBlazorServerApp";
    private string BaseUrl => NavManager.BaseUri;

    private AppUserDb _loggedInUser = new();
    
    // protected override async Task OnInitializedAsync()
    // {
    // }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await UpdateLoggedInUser();
    }

    private async Task UpdateLoggedInUser()
    {
        if (CurrentUserService.UserId is null)
            return;
        
        var foundUser = await UserRepository.GetByIdAsync((Guid)CurrentUserService.UserId!);
        if (!foundUser.Success)
            return;

        _loggedInUser = foundUser.Result!;
    }

    private async Task LogoutUser()
    {
        await AccountService.LogoutGuiAsync();
        // NavManager.NavigateTo(AppRouteConstants.Index, true);
    }
}