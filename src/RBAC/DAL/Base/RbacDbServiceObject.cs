using RBAC.RBAC.DAL.Enum;

namespace RBAC.RBAC.DAL.Base;

public class RbacDbServiceObject
{
    public required string ConnectionString { get; set; }

    public required RbacDbType DbType { get; set; }
}
