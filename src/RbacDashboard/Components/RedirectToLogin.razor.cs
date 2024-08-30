using Microsoft.AspNetCore.Components;
using RbacDashboard.Common.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components;


[ExcludeFromCodeCoverage]
public partial class RedirectToLogin
{
    #region Inject
    #pragma warning disable CS8618
    [Inject]
    protected RbacSecurityService Security { get; set; }


    [Inject]
    protected NavigationManager NavigationManager { get; set; } 
    #pragma warning restore CS8618
    #endregion

    protected override void OnInitialized()
    {
        if (!Security.IsAuthenticated())
        {
            NavigationManager.NavigateTo("RbacLogin", true);
        }
        else
        {
            NavigationManager.NavigateTo("Unauthorized");
        }
    }
}