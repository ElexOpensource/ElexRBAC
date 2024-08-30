
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using System.Text.Json.Nodes;

namespace RbacDashboard.DAL.Test;

public class GetRoleAccessesByRoleIdSqlHandlerTest : TestBase
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
            IsDeleted = false,
            Role = new Role
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.NewGuid(),
                RoleName = $"Role {i + 1}",
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
                AccessName = $"Access { i + 1}",
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
    public async Task Handle_ReturnsActiveRoleAccesses()
    {
        // Arrange
        using var context = CreateContext();
        var activeRole = context.RoleAccesses.First(ra => ra.IsActive);
        var handler = new GetRoleAccessesByRoleIdSqlHandler(context);
        var request = new GetRoleAccessesByRoleId(activeRole.RoleId, isActive: true, includeRole: true, includeAccess: true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Handle_ReturnsEmptyListForInactiveRoleAccesses()
    {
        // Arrange
        using var context = CreateContext();
        var inactiveRole = context.RoleAccesses.First(ra => !ra.IsActive);
        var handler = new GetRoleAccessesByRoleIdSqlHandler(context);
        var request = new GetRoleAccessesByRoleId(inactiveRole.RoleId, isActive: true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_IncludesRelatedEntities()
    {
        // Arrange
        using var context = CreateContext();
        var roleAccess = context.RoleAccesses.First();
        var handler = new GetRoleAccessesByRoleIdSqlHandler(context);
        var request = new GetRoleAccessesByRoleId(roleAccess.RoleId, includeRole: true, includeAccess: true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Role, Is.Not.Null);
        Assert.That(result.First().Access, Is.Not.Null);
        Assert.That(result.First().Role!.TypeMaster, Is.Not.Null);
        Assert.That(result.First().Access!.OptionsetMaster, Is.Not.Null);
    }

    [Test]
    public async Task Handle_DoesNotIncludeRelatedEntitiesWhenSpecified()
    {
        // Arrange
        using var context = CreateContext();
        var roleAccess = context.RoleAccesses.First();
        var handler = new GetRoleAccessesByRoleIdSqlHandler(context);
        var request = new GetRoleAccessesByRoleId(roleAccess.RoleId, includeRole: false, includeAccess: false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Role, Is.Null);
        Assert.That(result.First().Access, Is.Null);
    }
}
