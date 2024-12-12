using RBAC.RBAC.DAL.Enum;
using RBAC.RBAC.DAL.Models;

namespace RBAC.RBAC.Common.Interface;

public interface IRbacRoleRepository
{
    Task<List<Role>> GetByApplicationId(Guid applicationId, bool isActive);

    Task<List<Role>> GetAvailableParentsById(Guid applicationId, Guid roleId);

    Task<Role> GetById(Guid roleId);

    Task<Role> AddorUpdate(Role role);

    Task Delete(Guid roleId);

    Task ChangeStatus(Guid roleId, RecordStatus status);
}

