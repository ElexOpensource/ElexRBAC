using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common.ClientService;
using RbacDashboard.DAL.Models;
using Radzen;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Forms;

[ExcludeFromCodeCoverage]
public partial class AddEditApplication
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

    [Parameter]
    public Guid CustomerId { get; set; }
    #endregion

    #region Variables
    protected Application Application { get; set; } = new Application();
    protected string PageTittle { get; set; } = "Add Application";
    protected bool ErrorVisible { get; set; } = false;
    #endregion

    protected override async Task OnInitializedAsync()
    {
        if (Id != Guid.Empty)
        {
            PageTittle = "Edit Application";
            await GetApplication(Id);
        }
        else
        {
            PageTittle = "Add Application";
            Application = new Application
            {
                CustomerId = CustomerId,
                IsActive = true,
                IsDeleted = false,
                CreatedOn = DateTime.Now
            };
        }
    }

    protected async Task GetApplication(Guid Id)
    {
        try
        {
            Application = await ApiService.GetApplicationById(Id);

            if(Application == null)
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Warning, Detail = $"Application not found" });
        }
        catch
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load application" });
            DialogService.Close(Application);
        }
    }

    protected async Task FormSubmit()
    {
        try
        {
            var process = Application.Id != Guid.Empty ? "updateed" : "created";
            await ApiService.AddorUpdateApplication(Application);
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"Application {process} sucessfully" });
            DialogService.Close(Application);
        }
        catch
        {
            ErrorVisible = true;
        }
    }

    protected void Cancel(MouseEventArgs args)
    {
        DialogService.Close(null);
    }
}