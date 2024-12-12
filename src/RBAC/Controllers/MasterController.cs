using Microsoft.AspNetCore.Mvc;
using RBAC.RBAC.Common.Interface;
using RBAC.RBAC.DAL.Models.Domain;
using RBAC.RBAC.DAL.Models;

namespace RBAC.Controllers
{
    [Route("/Rbacapi/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Rbac")]
    public partial class MasterController(IRbacMasterRepository MasterRepository) : Controller
    {
        private readonly IRbacMasterRepository _master = MasterRepository;

        [HttpGet]
        public async Task<string> GenerateCustomerToken(Guid customerId)
        {
            return await _master.GenetrateTokenByCustomer(customerId);
        }

        [HttpGet]
        public async Task<List<TypeMaster>> GetTypeMasters()
        {
            return await _master.GetTypeMasters();
        }

        [HttpGet]
        public async Task<List<OptionsetMaster>> GetOptionsetMasters()
        {
            return await _master.GetOptionsetMasters();
        }

        [HttpGet]
        public async Task<List<Permissionset>> GetPermissionSets()
        {
            return await _master.GetPermissionSetList();
        }

        [HttpGet]
        public async Task<List<Option>> GetOptions(Guid applicationId, string optionName)
        {
            return await _master.GetOptions(applicationId, optionName);
        }
    }
}
