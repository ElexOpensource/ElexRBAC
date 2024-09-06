using Radzen;
using Radzen.Blazor;
using RbacDashboard.DAL.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common.ClientService;
using RbacDashboard.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Pages;

[ExcludeFromCodeCoverage]
public partial class Roleaccesses
{
    #region Inject
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    protected DialogService DialogService { get; set; }

    [Inject]
    protected NotificationService NotificationService { get; set; }

    [Inject]
    protected RbacApiService ApiService { get; set; }

    [Inject]
    protected RbacSecurityService Security { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    #region Variables

    public string ApplicationId { get; set; } = string.Empty;

    protected RadzenDataGrid<RoleAccess> AccessGrid { get; set; } = new RadzenDataGrid<RoleAccess>();

    protected IEnumerable<RoleAccess> RoleAccessesList { get; set; } = [];

    protected IEnumerable<RoleAccess> FilteredItems { get; set; } = [];

    protected IEnumerable<Role> RolesList { get; set; } = [];

    protected int Count { get; set; } = 0;
    protected int RoleCount { get; set; } = 0;

    protected bool IsLoading { get; set; } = false;

    protected Role? SelectedRole { get; set; }

    protected Guid SelectedRoleId { get; set; }

    #endregion

    protected override async Task OnInitializedAsync()
    {
        ApplicationId = Security.GetApplicationId();
        await LoadRolesData();
        await LoadAccessData();
    }

    protected async Task LoadRolesData()
    {
        try
        {
            IsLoading = true;
            RolesList = await GetRoles();
            SelectedRole = RolesList?.FirstOrDefault();
            SelectedRoleId = SelectedRole == null || SelectedRole.Id == Guid.Empty ? Guid.Empty : SelectedRole.Id;
            RoleCount = RolesList != null ?  RolesList.Count() : 0;
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Roles" });
        }
        finally 
        { 
            IsLoading = false; 
        }
    }

    protected async Task LoadAccessData()
    {
        try
        {
            IsLoading = true;
            RoleAccessesList = SelectedRoleId != Guid.Empty ? await GetRoleAccessByRoleId() : [];
            FilteredItems = RoleAccessesList;
            Count = FilteredItems.Count();
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load accesses for the selected role" });
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task RefreshGrid(MouseEventArgs args)
    {
        FilteredItems = [];
        RoleAccessesList = [];
        Count = 0;
        await LoadAccessData();
    }

    protected async Task AddRow(MouseEventArgs args)
    {
        await DialogService.OpenAsync<AddRoleaccess>("Add/Update Access", 
            new Dictionary<string, object> { 
                { "RoleId", SelectedRoleId }, 
                { "RoleName", RolesList.FirstOrDefault(role => role.Id == SelectedRoleId)?.RoleName ?? string.Empty }, 
                { "ApplicationId", Guid.Parse(ApplicationId) } ,
                { "RoleAccessList", RoleAccessesList }
            });
        await LoadAccessData();
    }
    
    protected async Task EditRow(RoleAccess roleAccess)
    {
        await DialogService.OpenAsync<EditRoleaccess>("Edit Role Access",
            new Dictionary<string, object> {
                { "ApplicationId", Guid.Parse(ApplicationId) } ,
                { "Roleaccess", roleAccess }
            });
        await LoadAccessData();
    }

    protected async Task DeleteRow(MouseEventArgs args, RoleAccess roleaccess)
    {
        try
        {
            if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
            {
                if (roleaccess.Id != Guid.Empty)
                {
                    await ApiService.DeleteRoleAccess(roleaccess.Id);
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"Access removed sucessfully" });
                    await LoadAccessData();
                }
                IsLoading = false;
            }
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = $"Error",
                Detail = $"Unable to delete roleaccess"
            });
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task Search(ChangeEventArgs args)
    {
        IsLoading = true;
        string[] columnNames = { "Role.RoleName", "CreatedOn", "Access.AccessName", "AccessMetaData" };
        FilteredItems = RoleAccessesList.Where(item => string.IsNullOrEmpty(args?.Value?.ToString()) ||
                columnNames.Any(column =>
                {
                    var parts = column.Split('.', 2);
                    var propertyValue = parts.Length >= 2 ?
                        item.GetType().GetProperty(parts[0])?.GetValue(item)?.GetType().GetProperty(parts[1])?.GetValue(item.GetType().GetProperty(parts[0])?.GetValue(item))?.ToString() :
                        item.GetType().GetProperty(column)?.GetValue(item)?.ToString();
                    return propertyValue?.IndexOf(args?.Value?.ToString()!, StringComparison.OrdinalIgnoreCase) >= 0;
                }))
            .ToList();
        Count = FilteredItems.Count();
        await AccessGrid.GoToPage(0);
        IsLoading = false;
    }

    protected async Task<List<Role>> GetRoles()
    {
        var roles = await ApiService.GetRoles(Guid.Parse(ApplicationId), true);

        if (roles.Count == 0)
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Info, Detail = $"Roles not available" });

        return roles;

    }

    protected async Task<List<RoleAccess>> GetRoleAccessByRoleId()
    {
        var roleAccess = await ApiService.GetRoleAccessByRoleId(SelectedRoleId);

        return roleAccess;
    }
}