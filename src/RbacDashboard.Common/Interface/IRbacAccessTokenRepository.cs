
namespace RbacDashboard.Common.Interface;

public interface IRbacAccessTokenRepository
{
    Task<string> GetByRoleIds(List<Guid> roleIds);
}
