﻿using System.Security.Claims;
using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Services.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Shared;

public partial class NavMenu
{
    // TODO: Find a working solution for MudTooltip to span full nav menu and only display when drawer is closed
    private bool _canViewApi;
    private bool _canViewJobs;
    private bool _canViewCounter;
    private bool _canViewWeather;
    
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
        var currentUser = await CurrentUserService.GetCurrentUserPrincipal();
        _canViewApi = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Api.View);
        _canViewJobs = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Jobs.View);
        _canViewCounter = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Example.Counter);
        _canViewWeather = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Example.Weather);
    }
}