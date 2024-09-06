using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Common.Interface;

public interface IRbacRoleRepository
{
    Task<List<Role>> GetByApplicationId(Guid applicationId, bool isActive);

    Task<Role> GetById(Guid roleId);

    Task<Role> AddorUpdate(Role roleId);

    Task Delete(Guid roleId);

    Task ChangeStatus(Guid roleId, RecordStatus status);
}