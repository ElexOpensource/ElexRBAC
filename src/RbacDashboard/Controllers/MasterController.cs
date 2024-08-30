using Microsoft.AspNetCore.Mvc;
using RbacDashboard.DAL.Models;
using RbacDashboard.Common.Interface;
using RbacDashboard.Common;
using RbacDashboard.DAL.Models.Domain;
using RbacDashboard.BAL;

namespace Rbac.Controllers;

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