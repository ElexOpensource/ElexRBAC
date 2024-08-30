using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using RbacDashboard.DAL.Models.Domain;
using Microsoft.AspNetCore.Authentication;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Authentication;

[ExcludeFromCodeCoverage]
public partial class ChangeApplication
{
    #region Inject
#pragma warning disable CS8618
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected SignInManager<RbacApplicationUser> SignInManager { get; set; }

    [Inject]
    protected RbacSecurityService Security { get; set; }
#pragma warning restore CS8618
    #endregion

    #region Parameter
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? ApplicationId { get; set; }
    #endregion

    protected override async Task OnInitializedAsync()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            await HttpContext.SignOutAsync(RbacConstants.AuthenticationSchema);
            if (ApplicationId != null)
            {
                var user = Security.User;
                user.ApplicationId = ApplicationId;
                await SignInManager.RefreshSignInAsync(user);
                NavigationManager.NavigateTo($"{NavigationManager.BaseUri}Roles", true);
            }
        }
    }
}
