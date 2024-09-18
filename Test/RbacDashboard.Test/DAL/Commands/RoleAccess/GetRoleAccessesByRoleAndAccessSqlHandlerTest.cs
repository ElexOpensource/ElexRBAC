using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class GetRoleAccessesByRoleAndAccessSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        SeedData(i => new RoleAccess
        {
            Id = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            AccessId = Guid.NewGuid(),
            IsActive = i % 2 == 0,
            IsDeleted = i % 3 == 0,
            Role = new Role
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                Name = $"Role {i + 1}",
                IsActive = i % 2 == 0,
                IsDeleted = false,
                TypeMaster = new TypeMaster
                {
                    Id = Guid.NewGuid(),
                    Name = $"Type {i + 1}",
                    IsActive = i % 2 == 0,
                    IsDeleted = false
                }
            },
            Access = new Access
            {
                Id = Guid.NewGuid(),
                Name = $"Access {i + 1}",
                ApplicationId = Guid.NewGuid(),
                IsActive = i % 2 == 0,
                IsDeleted = false,
                OptionsetMaster = new OptionsetMaster
                {
                    Id = Guid.NewGuid(),
                    Name = $"Optionset {i + 1}",
                    JsonObject = string.Empty,
                    IsActive = i % 2 == 0,
                    IsDeleted = false
                }
            }
        }, 10);
    }

    [Test]
    public async Task Handle_ReturnsActiveRoleAccess_WhenFilterActiveOnlyIsTrue()
    {
        // Arrange
        using var context = CreateContext();
        var activeRoleAccess = context.RoleAccesses.First(ra => ra.IsActive && !ra.IsDeleted);
        var handler = new GetRoleAccessesByRoleAndAccessSqlHandler(context);
        var request = new GetRoleAccessesByRoleAndAccessId(activeRoleAccess.RoleId, activeRoleAccess.AccessId, filterActiveOnly: true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsActive, Is.True);
        Assert.That(result.IsDeleted, Is.False);
    }

    [Test]
    public async Task Handle_ReturnsNull_WhenNoRoleAccessFound()
    {
        // Arrange
        using var context = CreateContext();
        var nonExistentRoleId = Guid.NewGuid();
        var nonExistentAccessId = Guid.NewGuid();
        var handler = new GetRoleAccessesByRoleAndAccessSqlHandler(context);
        var request = new GetRoleAccessesByRoleAndAccessId(nonExistentRoleId, nonExistentAccessId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_ReturnsInactiveRoleAccess_WhenFilterActiveOnlyIsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var inactiveRoleAccess = context.RoleAccesses.First(ra => !ra.IsActive);
        var handler = new GetRoleAccessesByRoleAndAccessSqlHandler(context);
        var request = new GetRoleAccessesByRoleAndAccessId(inactiveRoleAccess.RoleId, inactiveRoleAccess.AccessId, filterActiveOnly: false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsActive, Is.False);
        Assert.That(result.IsDeleted, Is.False);
    }

    [Test]
    public async Task Handle_ReturnsNull_WhenRoleAccessIsDeleted()
    {
        // Arrange
        using var context = CreateContext();
        var deletedRoleAccess = context.RoleAccesses.First(ra => ra.IsDeleted);
        var handler = new GetRoleAccessesByRoleAndAccessSqlHandler(context);
        var request = new GetRoleAccessesByRoleAndAccessId(deletedRoleAccess.RoleId, deletedRoleAccess.AccessId, filterActiveOnly: false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}
