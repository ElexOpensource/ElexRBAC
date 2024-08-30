using RbacDashboard.DAL.Models;
using RbacDashboard.Common;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Commands;

namespace RbacDashboard.BAL;

public class ApplicationRepository(IMediatorService mediator) : IRbacApplicationRepository
{
    private readonly IMediatorService _mediator = mediator;

    public async Task<Application> AddorUpdate(Application application)
    {
        return await _mediator.SendRequest(new AddorUpdateApplication(application));
    }

    public async Task Delete(Guid applicationId)
    {
        await _mediator.SendRequest(new DeleteApplication(applicationId));
    }

    public async Task<Application> GetById(Guid applicationId)
    {
        var application = await _mediator.SendRequest(new GetApplicationById(applicationId));

        if (application != null)
        {
            return application;
        }

        throw new KeyNotFoundException($"Application with id - {applicationId} is not available");
    }

    public async Task<List<Application>> GetByCustomerId(Guid customerId)
    {
        var applications = await _mediator.SendRequest(new GetApplicationByCustomerId(customerId));
        return applications;
    }
}