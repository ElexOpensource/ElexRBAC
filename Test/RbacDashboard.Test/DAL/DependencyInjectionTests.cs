using Microsoft.Extensions.DependencyInjection;
using RbacDashboard.DAL;
using RbacDashboard.DAL.Base;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Enum;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class DependencyInjectionTests : TestBase
{

    private IServiceCollection _services;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
    }

    [Test]
    public void AddDBService_AddsSqlService_WhenSqlDbTypeIsProvided()
    {
        // Arrange
        var dbServiceObject = new RbacDbServiceObject
        {
            ConnectionString = "Server=myServer;Database=myDb;User Id=myUser;Password=myPass;",
            DbType = RbacDbType.Sql
        };

        // Act
        _services.AddDBService(dbServiceObject);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<RbacSqlDbContext>();
        Assert.That(dbContext, Is.Not.Null, "RbacSqlDbContext should be registered.");
    }

    [Test]
    public void AddDBService_AddsPgSqlService_WhenPgSqlDbTypeIsProvided()
    {
        // Arrange
        var dbServiceObject = new RbacDbServiceObject
        {
            ConnectionString = "Host=myHost;Database=myDb;Username=myUser;Password=myPass;",
            DbType = RbacDbType.PgSql
        };

        // Act
        _services.AddDBService(dbServiceObject);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<RbacSqlDbContext>();
        Assert.That(dbContext, Is.Not.Null, "RbacSqlDbContext should not be registered for PgSql.");
    }

    [Test]
    public void AddDBService_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null;
        var dbServiceObject = new RbacDbServiceObject
        {
            ConnectionString = "Server=myServer;Database=myDb;User Id=myUser;Password=myPass;",
            DbType = RbacDbType.Sql
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddDBService(dbServiceObject));
    }

    [Test]
    public void AddDBService_ThrowsArgumentNullException_WhenDbServiceObjectIsNull()
    {
        // Arrange
        var dbServiceObject = (RbacDbServiceObject)null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _services.AddDBService(dbServiceObject));
    }

    [Test]
    public void AddDBService_ThrowsArgumentNullException_WhenConnectionStringIsNull()
    {
        // Arrange
        var dbServiceObject = new RbacDbServiceObject
        {
            ConnectionString = null,
            DbType = RbacDbType.Sql
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _services.AddDBService(dbServiceObject));
    }

    [Test]
    public void AddDBService_ThrowsInvalidOperationException_WhenDbTypeIsUnsupported()
    {
        // Arrange
        var dbServiceObject = new RbacDbServiceObject
        {
            ConnectionString = "Server=myServer;Database=myDb;User Id=myUser;Password=myPass;",
            DbType = (RbacDbType)999 // Unsupported database type
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _services.AddDBService(dbServiceObject));
    }
}
