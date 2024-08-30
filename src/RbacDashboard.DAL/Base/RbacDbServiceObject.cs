namespace RbacDashboard.DAL.Base;

public class RbacDbServiceObject
{
    public required string ConnectionString { get; set; }

    public required RbacDbType DbType { get; set; }
}

public enum RbacDbType
{
    Sql,
    PgSql
}