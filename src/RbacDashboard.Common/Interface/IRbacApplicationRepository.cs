using RbacDashboard.DAL.Models;

namespace RbacDashboard.Common.Interface;

public interface IRbacApplicationRepository
{
    Task<List<Application>> GetByCustomerId(Guid customerId);

    Task<Application> GetById(Guid applicationId);

    Task<Application> AddorUpdate(Application application);

    Task Delete(Guid applicationId);
}