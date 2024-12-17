using RBAC.RBAC.DAL.Enum;
using RBAC.RBAC.DAL.Models;

namespace RBAC.RBAC.Common.Interface;

public interface IRbacAccessRepository
{
    Task<List<Access>> GetByApplicationId(Guid applicationId, bool isActive);

    Task<Access> GetById(Guid accessId);

    Task<Access> AddorUpdate(Access access);

    Task Delete(Guid accessId);

    Task ChangeStatus(Guid accessId, RecordStatus status);
}

