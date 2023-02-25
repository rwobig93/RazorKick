using Microsoft.AspNetCore.Components;

namespace TestBlazorServerApp.Shared;

public partial class NavMenu
{
    [Parameter] public bool DisplayTooltips { get; set; } = true;
}