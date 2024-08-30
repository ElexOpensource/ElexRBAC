using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;

namespace RbacDashboard.Common.Interface;

public interface IRbacMasterRepository
{
    Task<string> GenetrateTokenByCustomer(Guid customerId);

    Task<List<TypeMaster>> GetTypeMasters();

    Task<List<OptionsetMaster>> GetOptionsetMasters();

    Task<List<Permissionset>> GetPermissionSetList();

    Task<List<Option>> GetOptions(Guid applicationId, string optionName);
}
