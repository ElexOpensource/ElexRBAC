using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class GetAccessesByApplicationIdSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_ValidRequest_ReturnsAccesses()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = Guid.NewGuid();
        var accesses = new List<Access>
        {
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 1", IsActive = true, IsDeleted = false },
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 2", IsActive = true, IsDeleted = false }
        };
        context.Accesses.AddRange(accesses);
        await context.SaveChangesAsync();

        var handler = new GetAccessesByApplicationIdSqlHandler(context);
        var request = new GetAccessesByApplicationId(applicationId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(a => a.ApplicationId == applicationId), Is.True);
    }

    [Test]
    public async Task Handle_NonExistentApplicationId_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetAccessesByApplicationIdSqlHandler(context);
        var request = new GetAccessesByApplicationId(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_InactiveAccesses_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = Guid.NewGuid();
        var accesses = new List<Access>
        {
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 1", IsActive = false, IsDeleted = false },
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 2", IsActive = false, IsDeleted = false }
        };
        context.Accesses.AddRange(accesses);
        await context.SaveChangesAsync();

        var handler = new GetAccessesByApplicationIdSqlHandler(context);
        var request = new GetAccessesByApplicationId(applicationId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_DeletedAccesses_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = Guid.NewGuid();
        var accesses = new List<Access>
        {
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 1", IsActive = true, IsDeleted = true },
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 2", IsActive = true, IsDeleted = true }
        };
        context.Accesses.AddRange(accesses);
        await context.SaveChangesAsync();

        var handler = new GetAccessesByApplicationIdSqlHandler(context);
        var request = new GetAccessesByApplicationId(applicationId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_IncludeOptionsetMaster_ReturnsAccessesWithOptionsetMaster()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = Guid.NewGuid();
        var optionsetMaster = new OptionsetMaster { Id = Guid.NewGuid(), Name = "Test Optionset", JsonObject = string.Empty };
        var accesses = new List<Access>
        {
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 1", IsActive = true, IsDeleted = false, OptionsetMasterId = optionsetMaster.Id },
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 2", IsActive = true, IsDeleted = false, OptionsetMasterId = optionsetMaster.Id }
        };
        context.Accesses.AddRange(accesses);
        context.OptionsetMasters.Add(optionsetMaster);
        await context.SaveChangesAsync();

        var handler = new GetAccessesByApplicationIdSqlHandler(context);
        var request = new GetAccessesByApplicationId(applicationId, true, true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(a => a.ApplicationId == applicationId), Is.True);

    }

    [Test]
    public async Task Handle_InactiveAccessesWithIsActiveFalse_ReturnsAccesses()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = Guid.NewGuid();
        var accesses = new List<Access>
        {
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 1", IsActive = false, IsDeleted = false },
            new Access { Id = Guid.NewGuid(), ApplicationId = applicationId, Name = "Access 2", IsActive = false, IsDeleted = false }
        };
        context.Accesses.AddRange(accesses);
        await context.SaveChangesAsync();

        var handler = new GetAccessesByApplicationIdSqlHandler(context);
        var request = new GetAccessesByApplicationId(applicationId, false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(a => a.ApplicationId == applicationId), Is.True);

    }
}
