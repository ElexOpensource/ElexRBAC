using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using RbacDashboard.Asserts;

namespace RbacDashboard.Components;

[ExcludeFromCodeCoverage]
public partial class App
{
    [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected NavigationManager NavigationManager { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private IComponentRenderMode? RenderModeForPage =>
        HttpContext.Request.Path.StartsWithSegments("/RbacLogin")
            ? null
            : new InteractiveServerRenderMode(prerender: false);

    private readonly string assemblyName = $"_content/{typeof(RbacAsserts).Assembly.GetName().Name}";
}
