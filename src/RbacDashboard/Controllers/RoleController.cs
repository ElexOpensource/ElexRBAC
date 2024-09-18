using Microsoft.AspNetCore.Mvc;
using RbacDashboard.DAL.Models;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Enum;

namespace Rbac.Controllers;

[Route("/Rbacapi/[controller]/[action]")]
[ApiExplorerSettings(GroupName = "Rbac")]
public partial class RoleController(IRbacRoleRepository RoleRepository) : Controller
{
    private readonly IRbacRoleRepository _role = RoleRepository;

    [HttpGet]
    public async Task<Role> GetById(Guid roleId)
    {
        if(roleId == Guid.Empty) throw new ArgumentNullException(nameof(roleId));
        return await _role.GetById(roleId);
    }

    [HttpGet]
    public async Task<List<Role>> GetByApplicationId(Guid applicationId, bool isActive = true)
    {
        if (applicationId == Guid.Empty) throw new ArgumentNullException(nameof(applicationId));
        return await _role.GetByApplicationId(applicationId, isActive);
    }

    [HttpGet]
    public async Task<List<Role>> GetAvailableParentsById(Guid applicationId, Guid roleId)
    {
        if (applicationId == Guid.Empty) throw new ArgumentNullException(nameof(applicationId));
        return await _role.GetAvailableParentsById(applicationId, roleId);
    }

    [HttpPost]
    public async Task<Role> AddorUpdate([FromBody] Role role)
    {
        return await _role.AddorUpdate(role);
    }

    [HttpPost]
    public async Task<OkResult> ChangeStatus(Guid roleId, RecordStatus status)
    {
        if (roleId == Guid.Empty) throw new ArgumentNullException(nameof(roleId));
        await _role.ChangeStatus(roleId, status);
        return Ok();
    }

    [HttpDelete]
    public async Task<OkResult> Delete(Guid roleId)
    {
        if (roleId == Guid.Empty) throw new ArgumentNullException(nameof(roleId));
        await _role.Delete(roleId);
        return Ok();
    }
}