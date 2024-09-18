using Microsoft.Extensions.DependencyInjection;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Base;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class DbServiceRegistrarTests : TestBase
{
    [Test]
    public void AddSqlService_RegistersSqlDbContextAndHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        var connectionString = "Server=localhost;Database=testdb;User Id=test;Password=test;";

        // Act
        DbServiceRegistrar.AddSqlService(services, connectionString);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<DbContextOptions<RbacSqlDbContext>>();
        var sqlServerDbContext = serviceProvider.GetRequiredService<DbContextOptions<SqlServerDbContext>>();

        Assert.That(dbContext, Is.Not.Null);
        Assert.That(sqlServerDbContext, Is.Not.Null);
    }

    [Test]
    public void AddPgSqlService_RegistersPgSqlDbContextAndHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        var connectionString = "Host=localhost;Database=testdb;Username=test;Password=test;";

        // Act
        DbServiceRegistrar.AddPgSqlService(services, connectionString);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<DbContextOptions<RbacSqlDbContext>>();
        var pgSqlServerDbContext = serviceProvider.GetRequiredService<DbContextOptions<PgSqlServerDbContext>>();

        Assert.That(dbContext, Is.Not.Null);
        Assert.That(pgSqlServerDbContext, Is.Not.Null);

    }

    [Test]
    public void AddSqlService_ThrowsArgumentNullException_ForNullServices()
    {
        // Arrange
        IServiceCollection services = null;
        var connectionString = "Server=localhost;Database=testdb;User Id=test;Password=test;";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DbServiceRegistrar.AddSqlService(services, connectionString));
    }

    [Test]
    public void AddPgSqlService_ThrowsArgumentNullException_ForNullServices()
    {
        // Arrange
        IServiceCollection services = null;
        var connectionString = "Host=localhost;Database=testdb;Username=test;Password=test;";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DbServiceRegistrar.AddPgSqlService(services, connectionString));
    }

    [Test]
    public void AddSqlService_ThrowsArgumentNullException_ForNullConnectionString()
    {
        // Arrange
        var services = new ServiceCollection();
        string connectionString = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DbServiceRegistrar.AddSqlService(services, connectionString));
    }

    [Test]
    public void AddPgSqlService_ThrowsArgumentNullException_ForNullConnectionString()
    {
        // Arrange
        var services = new ServiceCollection();
        string connectionString = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DbServiceRegistrar.AddPgSqlService(services, connectionString));
    }
}
