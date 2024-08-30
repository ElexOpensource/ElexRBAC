
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class GetRoleByIdSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        SeedData(i => new Role
        {
            Id = Guid.NewGuid(),
            RoleName = $"Role {i + 1}",
            IsActive = i % 2 == 0,
            IsDeleted = i % 3 == 0
        }, 10);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsRole()
    {
        // Arrange
        using var context = CreateContext();
        var activeRole = context.Roles.First(role => role.IsActive && !role.IsDeleted);

        var handler = new GetRoleByIdSqlHandler(context);
        var request = new GetRoleById(activeRole.Id);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(activeRole.Id));
    }

    [Test]
    public async Task Handle_InactiveRole_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var inactiveRole = context.Roles.First(role => !role.IsActive && !role.IsDeleted);

        var handler = new GetRoleByIdSqlHandler(context);
        var request = new GetRoleById(inactiveRole.Id);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_NonExistentRole_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var nonExistentRoleId = Guid.NewGuid();

        var handler = new GetRoleByIdSqlHandler(context);
        var request = new GetRoleById(nonExistentRoleId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_DeletedRole_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var deletedRole = context.Roles.First(role => role.IsDeleted);

        var handler = new GetRoleByIdSqlHandler(context);
        var request = new GetRoleById(deletedRole.Id);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}
