using Radzen;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;
using Microsoft.AspNetCore.Components;
using RbacDashboard.Common.ClientService;
using Microsoft.AspNetCore.Components.Web;
using RbacDashboard.Common.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Forms;

[ExcludeFromCodeCoverage]
public partial class AddRoleaccess
{
    #region Inject
#pragma warning disable CS8618
    [Inject]
    protected RbacApiService ApiService { get; set; }

    [Inject]
    protected DialogService DialogService { get; set; }

    [Inject]
    protected NotificationService NotificationService { get; set; }

    [Inject]
    protected RbacSecurityService Security { get; set; }
#pragma warning restore CS8618
    #endregion

    #region Parameter
    [Parameter]
    public Guid ApplicationId { get; set; }

    [Parameter]
    public string RoleName { get; set; } = string.Empty;

    [Parameter]
    public Guid RoleId { get; set; }

    [Parameter]
    public IEnumerable<RoleAccess> RoleAccessList { get; set; } = [];
    #endregion

    #region Variables

    protected IList<Access> AccessList { get; set; } = [];

    protected IList<Guid> SelectedAccess { get; set; } = [];

    protected Role Role { get; set; } = new Role();

    protected bool ErrorVisible { get; set; } = false;

    protected IList<Guid> DefaultSelectedAccess { get; set; } = [];

    #endregion

    protected override async Task OnInitializedAsync()
    {
        DefaultSelectedAccess = RoleAccessList.Select(ra => ra.AccessId).ToList();

        Role.Id = RoleId;
        Role.ApplicationId = ApplicationId;
        Role.RoleName = RoleName;

        SelectedAccess = DefaultSelectedAccess;
        AccessList = await ApiService.GetAccesses(ApplicationId, true);
    }

    protected async Task FormSubmit()
    {
        try
        {
            var removeAccesses = RoleAccessList
                .Where(ra => !SelectedAccess.Contains(ra.AccessId))
                .Select(ra => ra.AccessId)
                .ToList();

            var addAccesses = SelectedAccess
                .Where(sa => !DefaultSelectedAccess.Contains(sa))
                .ToList();

            await ApiService.AddRemoveAccessByRole(ApplicationId ,new AddRemoveAccessRequest { RoleId = RoleId, AddAccess = addAccesses, RemoveAccess = removeAccesses});
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"Role Access updated sucessfully" });
            DialogService.Close(Role);
        }
        catch
        {
            ErrorVisible = true;
        }
    }

    protected void Cancel(MouseEventArgs args) =>
        DialogService.Close(null);
}