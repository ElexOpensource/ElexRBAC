using Microsoft.AspNetCore.Mvc;
using RbacDashboard.DAL.Models;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Enum;

namespace Rbac.Controllers;

[Route("/Rbacapi/[controller]/[action]")]
[ApiExplorerSettings(GroupName = "Rbac")]
public partial class AccessController(IRbacAccessRepository accessRepository) : Controller
{
    private readonly IRbacAccessRepository _access = accessRepository;

    [HttpGet]
    public async Task<Access> GetById(Guid accessId)
    {
        if(accessId == Guid.Empty) throw new ArgumentNullException(nameof(accessId));
        return await _access.GetById(accessId);
    }

    [HttpGet]
    public async Task<List<Access>> GetByApplicationId(Guid applicationId, bool isActive = true)
    {
        if (applicationId == Guid.Empty) throw new ArgumentNullException(nameof(applicationId));
        return await _access.GetByApplicationId(applicationId, isActive);
    }

    [HttpPost]
    public async Task<Access> AddorUpdate([FromBody] Access access)
    {
        return await _access.AddorUpdate(access);
    }

    [HttpPost]
    public async Task<OkResult> ChangeStatus(Guid accessId, RecordStatus status)
    {
        if (accessId == Guid.Empty) throw new ArgumentNullException(nameof(accessId));
        await _access.ChangeStatus(accessId, status);
        return Ok();
    }

    [HttpDelete]
    public async Task<OkResult> Delete(Guid accessId)
    {
        if (accessId == Guid.Empty) throw new ArgumentNullException(nameof(accessId));
        await _access.Delete(accessId);
        return Ok();
    }
}