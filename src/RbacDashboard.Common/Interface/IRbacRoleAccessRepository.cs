using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;

namespace RbacDashboard.Common.Interface;

public interface IRbacRoleAccessRepository
{
    Task<List<RoleAccess>> GetByRoleId(Guid customerId);

    Task AddRemoveAccess(Guid applicationId, AddRemoveAccessRequest addRemoveAccessRequest);

    Task<RoleAccess> AddorUpdate(RoleAccess roleAccess);

    Task Delete(Guid roleAccessId);
}