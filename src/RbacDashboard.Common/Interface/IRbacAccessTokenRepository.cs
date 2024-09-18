using RbacDashboard.DAL.Enum;

namespace RbacDashboard.Common.Interface;

public interface IRbacAccessTokenRepository
{
    Task<string> GetByRoleIds(List<Guid> roleIds);

    Task<bool> DataMigration(string masterData, RbacTable table);
}
