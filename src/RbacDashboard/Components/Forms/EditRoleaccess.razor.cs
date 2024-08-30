using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common.ClientService;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Forms;

[ExcludeFromCodeCoverage]
public partial class EditRoleaccess
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
    public RoleAccess Roleaccess { get; set; } = new RoleAccess();
    #endregion

    #region Variable

    protected bool ErrorVisible { get; set; } = false;

    protected bool IsCanInHerit { get; set; }

    protected bool IsGenerateToken { get; set; }

    protected IEnumerable<Guid>? MetaDataValues { get; set; }

    protected List<Permissionset>? PermissionSet { get; set; }

    protected List<Option>? PermissionItems { get; set; } 
    #endregion

    protected override async Task OnInitializedAsync()
    {
        if (Roleaccess is null)
        {
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Detail = $"Unable to edit this record" });
            Cancel();
            return;
        }
        var metaDataObject = Roleaccess.AccessMetaData ?? Roleaccess.Access?.MetaData;
        var metaData = JsonConvert.DeserializeObject<AccessJSON>(metaDataObject ?? "");
        MetaDataValues = metaData?.Permissions?.ConvertAll(p => p.Id);

        if (metaData != null)
        {
            IsCanInHerit = metaData.CanInherit;
            IsGenerateToken = metaData.GenerateToken;
        }

        PermissionSet = await ApiService.GetPermissionSetList();
        PermissionItems = PermissionSet.Select(p => new Option { Id = p.Id, Label = p.Name }).ToList();
    }

    protected void UpdateMetaDataValues(IEnumerable<Guid> selectedValues)
    {
        MetaDataValues = selectedValues;
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
                    Label = PermissionSet?.FirstOrDefault(set => set?.Id == x)?.Name ?? string.Empty
                }).ToList() ?? [],

                CanInherit = IsCanInHerit,
                GenerateToken = IsGenerateToken
            };

            Roleaccess.AccessMetaData = JsonConvert.SerializeObject(metaData);
            await ApiService.AddorUpdateRoleAccess(Roleaccess);
            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Duration = 2000, Detail = $"Role access updated sucessfully" });
            DialogService.Close(Roleaccess);
        }
        catch
        {
            ErrorVisible = true;
        }
    }

    protected void Cancel() =>
        DialogService.Close(null);
    
}