using RbacDashboard.DAL.Models;

namespace RbacDashboard.Common.Interface;

public interface IRbacRoleRepository
{
    Task<List<Role>> GetByApplicationId(Guid customerId);

    Task<Role> GetById(Guid applicationId);

    Task<Role> AddorUpdate(Role application);

    Task Delete(Guid applicationId);
}