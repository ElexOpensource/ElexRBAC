using RBAC.RBAC.DAL.Enum;
using RBAC.RBAC.DAL.Models;

namespace RBAC.RBAC.Common.Interface;


public interface IRbacApplicationRepository
{
    Task<List<Application>> GetByCustomerId(Guid customerId, bool isActive);

    Task<Application> GetById(Guid applicationId);

    Task<Application> AddorUpdate(Application application);

    Task Delete(Guid applicationId);

    Task ChangeStatus(Guid applicationId, RecordStatus status);
}
