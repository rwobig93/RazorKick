using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Pages;

public partial class Index
{
    [Inject] private IConfiguration Configuration { get; set; } = null!;

    private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name ?? "TestBlazorServerApp";
    private string BaseAddress => Configuration.GetSection("AppConfiguration").GetSection("BaseUrl").Value;
}