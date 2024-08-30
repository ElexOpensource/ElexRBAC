using RbacDashboard.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using RbacDashboard.DAL.Base;

namespace RbacDashboard.DAL;

/// <summary>
/// Provides extension methods for setting up database services and applying migrations with seeding master data.
/// </summary>
public static class DependencyInjection
{
    private static RbacDbType? _dbType;

    /// <summary>
    /// Adds the appropriate database service to the service collection based on the specified <see cref="RbacDbServiceObject"/>.
    /// </summary>
    /// <param name="services">The service collection to add the database service to.</param>
    /// <param name="dbServiceObject">An object containing database connection information and type.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any required argument is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the specified database type is unsupported.</exception>
    public static IServiceCollection AddDBService([NotNull] this IServiceCollection services, [NotNull] RbacDbServiceObject dbServiceObject)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(dbServiceObject);
        ArgumentNullException.ThrowIfNull(dbServiceObject.ConnectionString);
        ArgumentNullException.ThrowIfNull(dbServiceObject.DbType);

        _dbType = dbServiceObject.DbType;

        switch (dbServiceObject.DbType)
        {
            case RbacDbType.Sql:
                DbServiceRegistrar.AddSqlService(services, dbServiceObject.ConnectionString);
                break;

            case RbacDbType.PgSql:
                DbServiceRegistrar.AddPgSqlService(services, dbServiceObject.ConnectionString);
                break;

            default:
                throw new InvalidOperationException("Unsupported database type");
        }

        return services;
    }

    /// <summary>
    /// Applies any pending database migrations and seeds the master data.
    /// </summary>
    /// <param name="app">The application builder used to configure the app's request pipeline.</param>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the database type is unsupported.</exception>
    [ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
    public static void ApplyMigrationsAndSeedData([NotNull] this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        DbContext context = _dbType switch
        {
            RbacDbType.Sql => services.GetRequiredService<SqlServerDbContext>(),
            RbacDbType.PgSql => services.GetRequiredService<PgSqlServerDbContext>(),
            _ => throw new InvalidOperationException("Unsupported database type")
        };

        var pendingMigrations = context.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Count == 0)
        {
            Console.WriteLine("No pending migrations found.");
        }
        else
        {
            Console.WriteLine("Applying migrations...");
            context.Database.Migrate();
            Console.WriteLine("Migrations completed.");
        }

        MasterDataSeeder.SeedMasterData(context);
    }
}
