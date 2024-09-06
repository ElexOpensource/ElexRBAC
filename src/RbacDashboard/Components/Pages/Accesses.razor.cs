using Radzen;
using Radzen.Blazor;
using RbacDashboard.DAL.Models;
using RbacDashboard.Components.Forms;
using Microsoft.AspNetCore.Components;
using RbacDashboard.Common.ClientService;
using RbacDashboard.Common.Authentication;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;
using RbacDashboard.DAL.Enum;

namespace RbacDashboard.Components.Pages;

[ExcludeFromCodeCoverage]
public partial class Accesses
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

    protected IEnumerable<Access> AccessList { get; set; } = [];

    protected RadzenDataGrid<Access> AccessGrid { get; set; } = new RadzenDataGrid<Access>();

    protected int Count { get; set; } = 0;

    protected IEnumerable<Access> FilteredItems { get; set; } = [];

    protected bool IsActive { get; set; } = true;

    protected bool IsLoading { get; set; } = false;
    #endregion

    protected override async Task OnInitializedAsync()
    {
        ApplicationId = Security.GetApplicationId();
        await LoadGridData();
    }

    protected async Task LoadGridData()
    {
        try
        {
            IsLoading = true;
            AccessList = await ApiService.GetAccesses(Guid.Parse(ApplicationId), IsActive);
            FilteredItems = AccessList;
            foreach (var item in FilteredItems)
            {
                if (item.ParentId != null)
                {
                    item.Parent = FilteredItems.FirstOrDefault(role => role.Id == item.ParentId);
                }
            }
            Count = FilteredItems.Count();

        }
        catch
        {
            NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load accesses" });
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task RefreshGrid(MouseEventArgs args)
    {
        FilteredItems = [];
        AccessList = [];
        Count = 0;
        await LoadGridData();
    }

    protected async Task AddRow(MouseEventArgs args)
    {
        await DialogService.OpenAsync<AddEditAccessObject>("Add AccessObject");
        await LoadGridData();
    }
    
    protected async Task EditRow(Access args)
    {
        await DialogService.OpenAsync<AddEditAccessObject>("Edit AccessObject", new Dictionary<string, object> { { "Id", args.Id } });
        await LoadGridData();
    }

    protected async Task ChangeStatus(Access access, RecordStatus status)
    {
        try
        {
            if (await DialogService.Confirm($"Are you sure you want to {status.ToStatusString()} this record?") == true)
            {
                if (access.Id != Guid.Empty)
                {
                    IsLoading = true;
                    await ApiService.ChangeAccessStatus(access.Id, status);
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"{status.ToStatusString()} sucessfully" });
                    await LoadGridData();
                }
            }
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = $"Error",
                Detail = $"Unable to {status.ToStatusString()} this record"
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
        string[] columnNames = { "AccessName", "MetaData", "CreatedOn", "OptionsetMaster.Name" };
        FilteredItems = AccessList.Where(item => string.IsNullOrEmpty(args?.Value?.ToString()) ||
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
}