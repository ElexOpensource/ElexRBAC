using Microsoft.AspNetCore.Mvc;
using RbacDashboard.Common.Interface;
using SampleWebApp.Models;
using System.Diagnostics;

namespace SampleWebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SampleController(IRbacAccessTokenRepository rbacAccessTokenRepository) : Controller
    {
        [HttpPost]
        public async Task<string> GetByRoleIds([FromBody] List<Guid> roleIds)
        {
            return await rbacAccessTokenRepository.GetByRoleIds(roleIds);
        }
    }
}
