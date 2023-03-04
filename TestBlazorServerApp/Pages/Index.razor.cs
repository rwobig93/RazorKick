using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace TestBlazorServerApp.Pages;

public partial class Index
{
    [Inject] private IConfiguration Configuration { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name ?? "TestBlazorServerApp";
    private string BaseUrl => NavManager.BaseUri;
}