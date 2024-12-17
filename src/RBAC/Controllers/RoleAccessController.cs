using Microsoft.AspNetCore.Mvc;
using RBAC.RBAC.Common.Interface;
using RBAC.RBAC.DAL.Models.Domain;
using RBAC.RBAC.DAL.Models;

namespace RBAC.Controllers
{
    [Route("/Rbacapi/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Rbac")]
    public partial class RoleAccessController(IRbacRoleAccessRepository RoleAccessRepository) : Controller
    {
        private readonly IRbacRoleAccessRepository _roleAccess = RoleAccessRepository;

        [HttpGet]
        public async Task<List<RoleAccess>> GetByRoleId(Guid roleId)
        {
            if (roleId == Guid.Empty) throw new ArgumentNullException(nameof(roleId));
            return await _roleAccess.GetByRoleId(roleId);
        }

        [HttpPost]
        public async Task<OkResult> AddRemoveAccess(Guid applicationId, [FromBody] AddRemoveAccessRequest roleAccess)
        {
            await _roleAccess.AddRemoveAccess(applicationId, roleAccess);
            return Ok();
        }

        [HttpPost]
        public async Task<RoleAccess> AddorUpdate([FromBody] RoleAccess roleAccess)
        {
            return await _roleAccess.AddorUpdate(roleAccess);
        }

        [HttpDelete]
        public async Task<OkResult> Delete(Guid roleAccessId)
        {
            if (roleAccessId == Guid.Empty) throw new ArgumentNullException(nameof(roleAccessId));
            await _roleAccess.Delete(roleAccessId);
            return Ok();
        }
    }
}
