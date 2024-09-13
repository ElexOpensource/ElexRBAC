using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Common.ClientService;

[ExcludeFromCodeCoverage]
public class RbacApiService(IHttpClientFactory factory) : RbacClientBase(factory)
{
    private readonly string RbacApiBasePath = "Rbacapi";

    #region Master

    public async Task<List<TypeMaster>> GetTypeMaster()
    {
        var endpoint = $"/{RbacApiBasePath}/Master/GetTypeMasters";
        return await GetAsync<List<TypeMaster>>(endpoint);        
    }

    public async Task<List<Permissionset>> GetPermissionSetList()
    {
        var endpoint = $"/{RbacApiBasePath}/Master/GetPermissionSets";
        return await GetAsync<List<Permissionset>>(endpoint);
    }

    public async Task<List<OptionsetMaster>> GetOptionsetMaster()
    {
        var endpoint = $"/{RbacApiBasePath}/Master/GetOptionsetMasters";
        return await GetAsync<List<OptionsetMaster>>(endpoint);
    }
    #endregion

    #region Application
    public async Task<List<Application>> GetApplications(Guid customerId, bool isActive = true)
    {
        var endpoint = $"/{RbacApiBasePath}/Application/GetByCustomerId?customerId={customerId}&isActive={isActive}";
        return await GetAsync<List<Application>>(endpoint);
    }

    public async Task<Application> GetApplicationById(Guid applicationId)
    {
        var endpoint = $"/{RbacApiBasePath}/Application/GetById?applicationId={applicationId}";
        return await GetAsync<Application>(endpoint);
    }

    public async Task<Application> AddorUpdateApplication(Application application)
    {
        var response = await PostAsync<Application>($"/{RbacApiBasePath}/Application/AddorUpdate", application);
        return response;
    }

    public async Task ChangeApplicationStatus(Guid applicationId, RecordStatus status)
    {
        await PostAsync($"/{RbacApiBasePath}/Application/ChangeStatus?applicationId={applicationId}&status={status}", new object());
    }

    public async Task DeleteApplication(Guid id)
    {
        var endpoint = $"/{RbacApiBasePath}/Application/Delete?applicationId={id}";
        await DeleteAsync(endpoint);
    }

    #endregion

    #region Access
    public async Task<List<Access>> GetAccesses(Guid applicationId, bool isActive = true)
    {
        var endpoint = $"/{RbacApiBasePath}/Access/GetByApplicationId?applicationId={applicationId}&isActive={isActive}";
        return await GetAsync<List<Access>>(endpoint);
    }

    public async Task DeleteAccess(Guid id)
    {
        var endpoint = $"/{RbacApiBasePath}/Access/Delete?accessId={id}";
        await DeleteAsync(endpoint);
    }

    public async Task<Access> GetAccessById(Guid accessId)
    {
        var endpoint = $"/{RbacApiBasePath}/Access/GetById?accessId={accessId}";
        return await GetAsync<Access>(endpoint);
    }

    public async Task<Access> AddorUpdateAccess(Access access)
    {
        var response = await PostAsync<Access>($"/{RbacApiBasePath}/Access/AddorUpdate", access);
        return response;
    }

    public async Task ChangeAccessStatus(Guid accessId, RecordStatus status)
    {
        await PostAsync($"/{RbacApiBasePath}/Access/ChangeStatus?accessId={accessId}&status={status}", new object());
    }

    #endregion

    #region Role
    public async Task<List<Role>> GetRoles(Guid applicationId, bool isActive = true)
    {
        var endpoint = $"/{RbacApiBasePath}/Role/GetByApplicationId?applicationId={applicationId}&isActive={isActive}";
        return await GetAsync<List<Role>>(endpoint);
    }

    public async Task<List<Role>> GetAvailableRolesForParent(Guid applicationId, Guid roleId)
    {
        var endpoint = $"/{RbacApiBasePath}/Role/GetAvailableParentsById?applicationId={applicationId}&roleId={roleId}";
        return await GetAsync<List<Role>>(endpoint);
    }

    public async Task<Role> GetRolById(Guid roleId)
    {
        var endpoint = $"/{RbacApiBasePath}/Role/GetById?roleId={roleId}";
        return await GetAsync<Role>(endpoint);
    }

    public async Task<Role> AddorUpdateRole(Role role)
    {
        var response = await PostAsync<Role>($"/{RbacApiBasePath}/Role/AddorUpdate", role);
        return response;
    }

    public async Task DeleteRole(Guid id)
    {
        var endpoint = $"/{RbacApiBasePath}/Role/Delete?roleId={id}";
        await DeleteAsync(endpoint);
    }

    public async Task ChangeRoleStatus(Guid roleId, RecordStatus status)
    {
        await PostAsync($"/{RbacApiBasePath}/Role/ChangeStatus?roleId={roleId}&status={status}", new object());
    }
    #endregion

    #region Role Access
    public async Task<List<RoleAccess>> GetRoleAccessByRoleId(Guid roleId)
    {
        var endpoint = $"/{RbacApiBasePath}/RoleAccess/GetByRoleId?roleId={roleId}";
        return await GetAsync<List<RoleAccess>>(endpoint);
    }

    public async Task AddRemoveAccessByRole(Guid applicationId,AddRemoveAccessRequest addorRemoveRoleAccessRequest)
    {
       await PostAsync($"/{RbacApiBasePath}/RoleAccess/AddRemoveAccess?applicationId={applicationId}", addorRemoveRoleAccessRequest);
    }

    public async Task<RoleAccess> AddorUpdateRoleAccess(RoleAccess roleAccess)
    {
        var response = await PostAsync<RoleAccess>($"/{RbacApiBasePath}/RoleAccess/AddorUpdate", roleAccess);
        return response;
    }

    public async Task DeleteRoleAccess(Guid id)
    {
        var endpoint = $"/{RbacApiBasePath}/RoleAccess/Delete?roleAccessId={id}";
        await DeleteAsync(endpoint);
    }
    #endregion
}