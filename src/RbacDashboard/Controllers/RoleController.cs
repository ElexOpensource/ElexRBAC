using Microsoft.AspNetCore.Mvc;
using RbacDashboard.DAL.Models;
using RbacDashboard.Common.Interface;

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
    public async Task<List<Role>> GetByApplicationId(Guid applicationId)
    {
        if (applicationId == Guid.Empty) throw new ArgumentNullException(nameof(applicationId));
        return await _role.GetByApplicationId(applicationId);
    }

    [HttpPost]
    public async Task<Role> AddorUpdate([FromBody] Role role)
    {
        return await _role.AddorUpdate(role);
    }

    [HttpDelete]
    public async Task<OkResult> Delete(Guid roleId)
    {
        if (roleId == Guid.Empty) throw new ArgumentNullException(nameof(roleId));
        await _role.Delete(roleId);
        return Ok();
    }
}