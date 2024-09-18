using Microsoft.AspNetCore.Mvc;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Enum;

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

        [HttpGet]
        public async Task<bool> MigrateRBACData()
        {
            string baseDirectory = AppContext.BaseDirectory;
            string roleAccessFile = Path.Combine(baseDirectory, "MasterData", "RoleAccess.json");
            string roleFile = Path.Combine(baseDirectory, "MasterData", "Role.json");
            string customerFile = Path.Combine(baseDirectory, "MasterData", "Customer.json");
            string applicationFile = Path.Combine(baseDirectory, "MasterData", "Application.json");
            string accessFile = Path.Combine(baseDirectory, "MasterData", "Access.json");

            string customer = await fileHelper.ExtractJsonAsync(customerFile);
            string application = await fileHelper.ExtractJsonAsync(applicationFile);
            string role = await fileHelper.ExtractJsonAsync(roleFile);
            string access = await fileHelper.ExtractJsonAsync(accessFile);
            string roleAccess = await fileHelper.ExtractJsonAsync(roleAccessFile);

            await rbacAccessTokenRepository.DataMigration(customer, RbacTable.Customer);
            await rbacAccessTokenRepository.DataMigration(application, RbacTable.Application);
            await rbacAccessTokenRepository.DataMigration(role, RbacTable.Role);
            await rbacAccessTokenRepository.DataMigration(access, RbacTable.Access);
            await rbacAccessTokenRepository.DataMigration(roleAccess, RbacTable.RoleAccess);

            return true;
        }

    }

    public static class fileHelper
    {
        public static async Task<string> ExtractJsonAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
