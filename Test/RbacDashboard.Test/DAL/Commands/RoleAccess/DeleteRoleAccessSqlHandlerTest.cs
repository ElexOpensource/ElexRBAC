using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class DeleteRoleAccessSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        var context = CreateContext();
        SeedData(i => new RoleAccess
        {
            Id = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            AccessId = Guid.NewGuid(),
            IsActive = true,
            IsDeleted = false
        }, 5);
    }

    [Test]
    public async Task Handle_ExistingRoleAccess_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        var existingRoleAccess = context.RoleAccesses.First();
        var handler = new DeleteRoleAccessSqlHandler(context);
        var request = new DeleteRoleAccess(existingRoleAccess.Id);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(context.RoleAccesses.Find(existingRoleAccess.Id), Is.Null);
    }

    [Test]
    public async Task Handle_NonExistentRoleAccess_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new DeleteRoleAccessSqlHandler(context);
        var nonExistentRoleAccessId = Guid.NewGuid();
        var request = new DeleteRoleAccess(nonExistentRoleAccessId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Handle_ExistingRoleAccess_DeletesCorrectEntity()
    {
        // Arrange
        using var context = CreateContext();
        var existingRoleAccess = context.RoleAccesses.First();
        var handler = new DeleteRoleAccessSqlHandler(context);
        var request = new DeleteRoleAccess(existingRoleAccess.Id);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(context.RoleAccesses.Find(existingRoleAccess.Id), Is.Null);
        Assert.That(context.RoleAccesses.Any(ra => ra.Id == existingRoleAccess.Id), Is.False);
    }
}
