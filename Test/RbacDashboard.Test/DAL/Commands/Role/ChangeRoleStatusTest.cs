using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class ChangeRoleStatusTest : TestBase
{
    private ChangeRoleStatusSqlHandler _handler;

    [SetUp]
    public void Init()
    {
        var dbContext = CreateContext();
        _handler = new ChangeRoleStatusSqlHandler(dbContext);
    }

    [Test]
    public async Task Handle_WhenRoleNotFound_ReturnsFalse()
    {
        // Arrange
        var request = new ChangeRoleStatus(Guid.NewGuid(), RecordStatus.Active);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Handle_WhenRoleFoundAndStatusActive_SetsIsActiveTrue()
    {
        // Arrange
        var role = new Role { Id = Guid.NewGuid(), Name = "Role", IsActive = false };
        SeedData(new List<Role> { role });
        var request = new ChangeRoleStatus(role.Id, RecordStatus.Active);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedRole = await context.Roles.FindAsync(role.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedRole.IsActive, Is.True);
    }

    [Test]
    public async Task Handle_WhenRoleFoundAndStatusInactive_SetsIsActiveFalse()
    {
        // Arrange
        var Role = new Role { Id = Guid.NewGuid(), Name = "Role", IsActive = true };
        SeedData(new List<Role> { Role });
        var request = new ChangeRoleStatus(Role.Id, RecordStatus.Inactive);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedRole = await context.Roles.FindAsync(Role.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedRole.IsActive, Is.False);
    }

    [Test]
    public async Task Handle_WhenRoleFoundAndStatusDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var Role = new Role { Id = Guid.NewGuid(), Name = "Role", IsDeleted = false };
        SeedData(new List<Role> { Role });
        var request = new ChangeRoleStatus(Role.Id, RecordStatus.Delete);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedRole = await context.Roles.FindAsync(Role.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedRole.IsDeleted, Is.True);
    }
}
