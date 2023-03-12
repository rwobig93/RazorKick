using System.Reflection;
using Application.Constants.Web;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Microsoft.AspNetCore.Components;
using Shared.Responses.Identity;

namespace TestBlazorServerApp.Pages;

public partial class Index
{
    [Inject] private IAppUserRepository UserRepository { get; init; } = null!;
    [Inject] private IAppAccountService AccountService { get; init; } = null!;

    private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name ?? "TestBlazorServerApp";
    private string BaseUrl => NavManager.BaseUri;

    private UserBasicResponse _loggedInUser = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await UpdateLoggedInUser();
            StateHasChanged();
        }
    }

    private async Task UpdateLoggedInUser()
    {
        var user = await CurrentUserService.GetCurrentUserBasic();
        if (user is null)
            return;

        _loggedInUser = user;
    }
}