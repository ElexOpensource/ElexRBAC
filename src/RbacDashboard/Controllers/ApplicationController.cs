using Microsoft.AspNetCore.Mvc;
using RbacDashboard.DAL.Models;
using RbacDashboard.Common.Interface;

namespace Rbac.Controllers;

[Route("/Rbacapi/[controller]/[action]")]
[ApiExplorerSettings(GroupName = "Rbac")]
public partial class ApplicationController(IRbacApplicationRepository applicationRepository) : Controller
{
    private readonly IRbacApplicationRepository _application = applicationRepository;

    [HttpGet]
    public async Task<Application> GetById(Guid applicationId)
    {
        if(applicationId == Guid.Empty) throw new ArgumentNullException(nameof(applicationId));
        return await _application.GetById(applicationId);
    }

    [HttpGet]
    public async Task<List<Application>> GetByCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty) throw new ArgumentNullException(nameof(customerId));
        return await _application.GetByCustomerId(customerId);
    }

    [HttpPost]
    public async Task<Application> AddorUpdate([FromBody] Application application)
    {
        return await _application.AddorUpdate(application);
    }

    [HttpDelete]
    public async Task<OkResult> Delete(Guid applicationId)
    {
        if (applicationId == Guid.Empty) throw new ArgumentNullException(nameof(applicationId));
        await _application.Delete(applicationId);
        return Ok();
    }
}