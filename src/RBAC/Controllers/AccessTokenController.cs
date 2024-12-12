using Microsoft.AspNetCore.Mvc;
using RBAC.RBAC.Common.Interface;

namespace RBAC.Controllers
{
    [Route("/Rbacapi/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Rbac")]
    public partial class AccessTokenController(IRbacAccessTokenRepository accessTokenRepository) : Controller
    {
        private readonly IRbacAccessTokenRepository _token = accessTokenRepository;

        [HttpPost]
        public async Task<string> GetByRoleIds([FromBody] List<Guid> roleIds)
        {
            return await _token.GetByRoleIds(roleIds);
        }
    }
}
