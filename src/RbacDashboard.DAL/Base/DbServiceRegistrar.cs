using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Base;

/// <summary>
/// Provides methods for registering database services and handlers.
/// </summary>
public static class DbServiceRegistrar
{
    /// <summary>
    /// Registers the SQL Server database services and handlers.
    /// </summary>
    /// <param name="services">The service collection to add the database service to.</param>
    /// <param name="connectionString">The connection string for the SQL Server database.</param>
    /// <exception cref="ArgumentNullException">Thrown if any required argument is null.</exception>
    public static void AddSqlService([NotNull] IServiceCollection services, [NotNull] string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddDbContext<RbacSqlDbContext>(options => { options.UseSqlServer(connectionString); });

        services.AddDbContext<SqlServerDbContext>(options => { options.UseSqlServer(connectionString); });

        RegisterHandlers<ISqlHandler>(services);
    }

    /// <summary>
    /// Registers the PostgreSQL database services and handlers.
    /// </summary>
    /// <param name="services">The service collection to add the database service to.</param>
    /// <param name="connectionString">The connection string for the PostgreSQL database.</param>
    /// <exception cref="ArgumentNullException">Thrown if any required argument is null.</exception>
    public static void AddPgSqlService([NotNull] IServiceCollection services, [NotNull] string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(connectionString);
        services.AddDbContext<RbacSqlDbContext>(options => { options.UseNpgsql(connectionString); });
        services.AddDbContext<PgSqlServerDbContext>(options => { options.UseNpgsql(connectionString); });

        RegisterHandlers<ISqlHandler>(services);
    }

    /// <summary>
    /// Registers handlers that implement the specified interface.
    /// </summary>
    /// <typeparam name="T">The type of the interface that the handlers implement.</typeparam>
    /// <param name="services">The service collection to add the handlers to.</param>
    private static void RegisterHandlers<T>(IServiceCollection services)
    {
        var handlers = typeof(T).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(T)) && !t.IsAbstract);

        foreach (var handler in handlers)
        {
            var interfaces = handler.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var @interface in interfaces)
            {
                services.AddTransient(@interface, handler);
            }
        }
    }
}