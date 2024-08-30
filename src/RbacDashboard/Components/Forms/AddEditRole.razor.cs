using Radzen;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common.ClientService;
using RbacDashboard.DAL.Models;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Forms;

[ExcludeFromCodeCoverage]
public partial class AddEditRole
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
    public Guid Id { get; set; }
    #endregion

    #region Variable
    protected string PageTittle { get; set; } = "Add Role";

    protected string ApplicationId { get; set; } = string.Empty;

    protected bool ErrorVisible { get; set; } = false;

    protected Role Role { get; set; } = new Role();

    protected Role ParentsValue { get; set; } = new Role();

    protected TypeMaster RoleTypeValue { get; set; } = new TypeMaster();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected IEnumerable<TypeMaster> TypeMaster { get; set; }

    protected IEnumerable<Role> Parents { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected int RoleTypeCount { get; set; } = 0;

    protected int ParentsCount { get; set; } = 0;
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        ApplicationId = Security.GetApplicationId();

        if (Id != Guid.Empty)
        {
            PageTittle = "Edit Role";
            await GetRole();            
        }
        else
        {
            PageTittle = "Add Role";
            Role = new Role
            {
                IsDeleted = false,
                IsActive = true,
                ApplicationId = Guid.Parse(ApplicationId),
                CreatedOn = DateTime.Now
            };
        }
    }

    protected async Task GetRole()
    {
        try
        {
            Role = await ApiService.GetRolById(Id);
            if (Role is null)
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Warning, Detail = $"Role not found" });
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load role" });
            DialogService.Close(Role);
        }
    }

    protected async Task LoadRoleType(LoadDataArgs args)
    {
        try
        {
            TypeMaster = await ApiService.GetTypeMaster();
            RoleTypeCount = TypeMaster.Count();

            if (!Equals(Role.TypeMasterId, null))
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                RoleTypeValue = TypeMaster.FirstOrDefault(x => x.Id == Role.TypeMasterId);
#pragma warning restore CS8601 // Possible null reference assignment.
            }

            if (RoleTypeCount is 0)
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Warning, Detail = $"Type master not found" });

        }
        catch
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load types" });
        }
    }

    protected async Task LoadParents(LoadDataArgs args)
    {
        try
        {
            Parents = await ApiService.GetRoles(Role.ApplicationId);
            if (Id != Guid.Empty)
                Parents = Parents.Where(x => x.Id != Id).ToList();

            ParentsCount = Parents.Count();

            if (!Equals(Role.ParentId, null))
#pragma warning disable CS8601 // Possible null reference assignment.
                ParentsValue = Parents.FirstOrDefault(x => x.Id == Role.ParentId);
#pragma warning restore CS8601 // Possible null reference assignment.
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load roles" });
        }
    }

    protected async Task FormSubmit()
    {
        try
        {
            var process = Role.Id != Guid.Empty ? "Updateed" : "Created";
            await ApiService.AddorUpdateRole(Role);
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"Role {process} sucessfully" });
            DialogService.Close(Role);
        }
        catch
        {
            ErrorVisible = true;
        }
    }

    protected void Cancel(MouseEventArgs args) =>  DialogService.Close(null);    
}