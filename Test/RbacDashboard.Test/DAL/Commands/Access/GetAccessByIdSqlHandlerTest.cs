

using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class GetAccessByIdSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_ValidRequest_ReturnsAccess()
    {
        // Arrange
        using var context = CreateContext();
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId, AccessName = "Test Access", IsActive = true, IsDeleted = false };
        context.Accesses.Add(access);
        await context.SaveChangesAsync();

        var handler = new GetAccessByIdSqlHandler(context);
        var request = new GetAccessById(accessId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(accessId));
    }

    [Test]
    public async Task Handle_NonExistentAccess_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetAccessByIdSqlHandler(context);
        var request = new GetAccessById(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_InactiveAccess_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId, AccessName = "Test Access", IsActive = false, IsDeleted = false };
        context.Accesses.Add(access);
        await context.SaveChangesAsync();

        var handler = new GetAccessByIdSqlHandler(context);
        var request = new GetAccessById(accessId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_DeletedAccess_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId, AccessName = "Test Access", IsActive = true, IsDeleted = true };
        context.Accesses.Add(access);
        await context.SaveChangesAsync();

        var handler = new GetAccessByIdSqlHandler(context);
        var request = new GetAccessById(accessId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_ActiveAccess_ReturnsAccess()
    {
        // Arrange
        using var context = CreateContext();
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId, AccessName = "Active Access", IsActive = true, IsDeleted = false };
        context.Accesses.Add(access);
        await context.SaveChangesAsync();

        var handler = new GetAccessByIdSqlHandler(context);
        var request = new GetAccessById(accessId, true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(accessId));
        Assert.That(result.IsActive, Is.True);
    }

    [Test]
    public async Task Handle_InactiveAccessWithIsActiveFalse_ReturnsAccess()
    {
        // Arrange
        using var context = CreateContext();
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId, AccessName = "Inactive Access", IsActive = false, IsDeleted = false };
        context.Accesses.Add(access);
        await context.SaveChangesAsync();

        var handler = new GetAccessByIdSqlHandler(context);
        var request = new GetAccessById(accessId, false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null); 
        Assert.That(result.Id, Is.EqualTo(accessId));
        Assert.That(result.IsActive, Is.False);
    }
}
