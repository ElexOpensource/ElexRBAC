using Radzen;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common.ClientService;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Forms;

[ExcludeFromCodeCoverage]
public partial class AddEditAccessObject
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
    protected string PageTittle { get; set; } = "Add Access Object";

    protected string ApplicationId { get; set; } = string.Empty;

    protected bool IsCanInHerit { get; set; } = false;

    protected bool IsGenerateToken { get; set; } = false;

    protected bool ErrorVisible { get; set; } = false;

    protected Access Access { get; set; } = new Access{ AccessName = string.Empty, OptionsetMasterId = Guid.Empty };

    protected int OptionSetMastersCount { get; set; } = 0;

    protected OptionsetMaster AccessTypeValue { get; set; } = new OptionsetMaster();

    protected List<Permissionset>? PermissionSet { get; set; }

    protected List<Option>? PermissionItems { get; set; }

    protected IEnumerable<Guid>? MetaDataValues { get; set; }

    protected IEnumerable<OptionsetMaster>? OptionsetMasters { get; set; }
    #endregion

    protected override async Task OnInitializedAsync()
    {
        ApplicationId = Security.GetApplicationId();

        if (Id != Guid.Empty)
        {
            PageTittle = "Edit Access Object";
            Access = await ApiService.GetAccessById(Id);
            var metaData = JsonConvert.DeserializeObject<AccessJSON>(Access?.MetaData ?? "");
            MetaDataValues = metaData?.Permissions?.ConvertAll(p => p.Id);
            IsCanInHerit = metaData?.CanInherit != null && metaData.CanInherit;
            IsGenerateToken = metaData?.GenerateToken != null && metaData.GenerateToken;
        }
        else
        {
            PageTittle = "Add Access Object";
            Access = new Access
            {
                AccessName = string.Empty,
                IsActive = true,
                IsDeleted = false,
                ApplicationId = Guid.Parse(ApplicationId),
                CreatedOn = DateTime.Now
            };
        }
        PermissionSet = await ApiService.GetPermissionSetList();
        PermissionItems = PermissionSet.Select(p => new Option { Id = p.Id, Label = p.Name }).ToList();
    }

    protected async Task LoadAccessType(LoadDataArgs args)
    {
        try
        {
            OptionsetMasters = await ApiService.GetOptionsetMaster();

            if (!object.Equals(Access.OptionsetMasterId, null))
            {
                AccessTypeValue = OptionsetMasters.FirstOrDefault(x => x.Id == Access.OptionsetMasterId) ?? new OptionsetMaster();
            }
            OptionSetMastersCount = OptionsetMasters.Count();

            if(OptionSetMastersCount is 0)
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Warning, Detail = $"Type master not found" });

        }
        catch
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load option set master" });
        }
    }

    protected async Task FormSubmit()
    {
        try
        {
            var metaData = new AccessJSON
            {
                Permissions = MetaDataValues?.Select(x => new Option 
                { 
                    Id = x, 
                    Label = PermissionSet?.FirstOrDefault(set => set?.Id == x)?.Name  ?? string.Empty
                }).ToList() ?? [],

                CanInherit = IsCanInHerit,
                GenerateToken = IsGenerateToken
            };

            Access.MetaData = JsonConvert.SerializeObject(metaData);
            await ApiService.AddorUpdateAccess(Access);
            var process = Access.Id != Guid.Empty ? "Updateed" : "Created";
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"Access {process} sucessfully" });
            DialogService.Close(Access);
        }
        catch
        {
            ErrorVisible = true;
        }
    }

    protected void Cancel(MouseEventArgs args) =>
        DialogService.Close(null);
    
    protected void UpdateMetaDataValues(IEnumerable<Guid> selectedValues) => 
        MetaDataValues = selectedValues;
    
}