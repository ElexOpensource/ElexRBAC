using RBAC.RBAC.DAL.Models.Domain;
using RBAC.RBAC.DAL.Models;

namespace RBAC.RBAC.Common.Interface;

public interface IRbacMasterRepository
{
    Task<string> GenetrateTokenByCustomer(Guid customerId);

    Task<List<TypeMaster>> GetTypeMasters();

    Task<List<OptionsetMaster>> GetOptionsetMasters();

    Task<List<Permissionset>> GetPermissionSetList();

    Task<List<Option>> GetOptions(Guid applicationId, string optionName);
}

