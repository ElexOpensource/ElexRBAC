using RBAC.RBAC.DAL.Enum;

namespace RBAC.RBAC.Common.Interface;

public interface IRbacAccessTokenRepository
{
    Task<string> GetByRoleIds(List<Guid> roleIds);

    Task<bool> DataMigration(string masterData, RbacTable table);
}

