using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Data;

/// <summary>
/// use 'dotnet ef migrations add -c SqlServerDbContext SqlInitialCreate -p../RbacDashboard.DAL -o Migrations/SqlMigrations -s.' this commend to create migration script for SQL
/// </summary>
/// <param name="options"></param>
[ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
public class SqlServerDbContext(DbContextOptions<RbacSqlDbContext> options) : RbacSqlDbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(builder => builder.MigrationsAssembly("RbacDashboard.DAL"));
}


/// <summary>
/// use 'dotnet ef migrations add -c PgSqlServerDbContext PostgreSqlInitialCreate -p ../RbacDashboard.DAL -o Migrations/PostgreSqlMigrations -s .' this commend to create migration script for PgSql
/// </summary>
/// <param name="options"></param>
[ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
public class PgSqlServerDbContext(DbContextOptions<RbacSqlDbContext> options) : RbacSqlDbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql(builder => builder.MigrationsAssembly("RbacDashboard.DAL"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(Guid) && p.GetDefaultValueSql() != null))
        {
            property.SetDefaultValueSql("uuid_generate_v4()");
        }

        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(DateTimeOffset) && p.GetDefaultValueSql() != null))
        {
            property.SetDefaultValueSql("NOW()");
        }
    }
}