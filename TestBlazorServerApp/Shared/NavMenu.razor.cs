using System.Security.Claims;
using Application.Services.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Shared;

public partial class NavMenu
{
    [CascadingParameter(Name = "ApplicationName")]
    public string ApplicationName { get; set; } = "";
    
    [CascadingParameter(Name = "CurrentUser")]
    private ClaimsPrincipal CurrentUser { get; set; } = new();
    
    // TODO: Find a working solution for MudTooltip to span full nav menu and only display when drawer is closed
}