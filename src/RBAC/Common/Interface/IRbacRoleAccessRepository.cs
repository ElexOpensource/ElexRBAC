using RBAC.RBAC.DAL.Models.Domain;
using RBAC.RBAC.DAL.Models;

namespace RBAC.RBAC.Common.Interface;

public interface IRbacRoleAccessRepository
{
    Task<List<RoleAccess>> GetByRoleId(Guid customerId);

    Task AddRemoveAccess(Guid applicationId, AddRemoveAccessRequest addRemoveAccessRequest);

    Task<RoleAccess> AddorUpdate(RoleAccess roleAccess);

    Task Delete(Guid roleAccessId);
}

