using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using RBAC.RBAC.Common.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace RBAC.Components.Layout
{
    [ExcludeFromCodeCoverage]
    public partial class ApplicationLayout
    {
        #region Inject
#pragma warning disable CS8618
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected RbacSecurityService Security { get; set; }
#pragma warning restore CS8618
        #endregion

        #region Variables
        private bool SidebarExpanded { get; set; } = true;

        protected string BaseUrl { get; set; } = string.Empty;
        #endregion

        protected override void OnInitialized() => BaseUrl = NavigationManager.BaseUri;

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }
    }
}
