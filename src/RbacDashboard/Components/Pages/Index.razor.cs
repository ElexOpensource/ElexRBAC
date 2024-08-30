using Radzen;
using Microsoft.AspNetCore.Components;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common.ClientService;
using RbacDashboard.DAL.Models;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Web;
using RbacDashboard.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Pages;

[ExcludeFromCodeCoverage]
public partial class Index
{
    #region Inject
#pragma warning disable CS8618
    [Inject]
    protected RbacApiService ApiService { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected DialogService DialogService { get; set; }

    [Inject]
    protected NotificationService NotificationService { get; set; }

    [Inject]
    protected RbacSecurityService Security { get; set; }
#pragma warning restore CS8618
    #endregion

    #region Variables
    protected List<Application> ApplicationList { get; set; } = [];
    protected RadzenDataGrid<Application> DataGrid { get; set; } = new RadzenDataGrid<Application>();
    protected List<Application> FilteredItems { get; set; } = [];
    protected string CustomerId { get; set; } = string.Empty;
    protected int Count { get; set; } = 0;
    protected bool IsLoading { get; set; } = false;
    #endregion

    protected override async Task OnInitializedAsync()
    {
        CustomerId = Security.GetCustomerId();
        await LoadGridData();
    }

    protected async Task LoadGridData()
    {
        try
        {
            IsLoading = true;
            ApplicationList = await ApiService.GetApplications(Guid.Parse(CustomerId));
            FilteredItems = ApplicationList;
            Count = FilteredItems.Count;

            if(!ApplicationList.Any())
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Info, Detail = $"Applications not available" });
            
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load applications" });
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task RefreshGrid(MouseEventArgs args)
    {
        FilteredItems = [];
        ApplicationList = [];
        Count = 0;
        await LoadGridData();
    }

    protected async Task AddRow(MouseEventArgs args)
    {
        await DialogService.OpenAsync<AddEditApplication>("Add Application", new Dictionary<string, object> { { "CustomerId", Guid.Parse(CustomerId) } });
        await LoadGridData();
    }

    protected async Task EditRow(Application args)
    {
        await DialogService.OpenAsync<AddEditApplication>("Edit Application", new Dictionary<string, object> { { "Id", args.Id }, { "CustomerId", Guid.Parse(CustomerId) } });
        await LoadGridData();
    }

    protected async Task DeleteRow(MouseEventArgs args, Application application)
    {
        try
        {
            if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
            {
                IsLoading = true;
                if (application.Id != Guid.Empty)
                {
                    await ApiService.DeleteApplication(application.Id);
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"Application deleted sucessfully" });
                    await LoadGridData();
                }
                IsLoading = false;
            }
        }
        catch
        {
            IsLoading = false;
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = $"Error",
                Detail = $"Unable to delete application"
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
        string[] columnNames = { "ApplicationName", "CreatedOn" };
        FilteredItems = ApplicationList.Where(item => string.IsNullOrEmpty(args?.Value?.ToString()) ||
                columnNames.Any(column =>
                {
                    var parts = column.Split('.', 2);
                    var propertyValue = parts.Length >= 2 ?
                        item.GetType().GetProperty(parts[0])?.GetValue(item)?.GetType().GetProperty(parts[1])?.GetValue(item.GetType().GetProperty(parts[0])?.GetValue(item))?.ToString() :
                        item.GetType().GetProperty(column)?.GetValue(item)?.ToString();
                    return propertyValue?.IndexOf(args?.Value?.ToString()!, StringComparison.OrdinalIgnoreCase) >= 0;
                }))
            .ToList();
        Count = FilteredItems.Count;
        await DataGrid.GoToPage(0);
        IsLoading = false;
    }

    protected void SelectApplication(Application args)
    {
        if(args?.Id is not null)
            Security.ChangeApplication(args.Id);
    }
}