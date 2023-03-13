using System.Reflection;
using Application.Constants.Identity;
using Application.Helpers.Runtime;
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
    
    private bool _canViewApi;
    private bool _canViewJobs;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await UpdateLoggedInUser();
            await GetPermissions();
            StateHasChanged();
        }
    }

    private async Task GetPermissions()
    {
        var currentUser = await CurrentUserService.GetCurrentUserPrincipal();
        _canViewApi = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Api.View);
        _canViewJobs = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Jobs.View);
    }

    private async Task UpdateLoggedInUser()
    {
        var user = await CurrentUserService.GetCurrentUserBasic();
        if (user is null)
            return;

        _loggedInUser = user;
    }
}