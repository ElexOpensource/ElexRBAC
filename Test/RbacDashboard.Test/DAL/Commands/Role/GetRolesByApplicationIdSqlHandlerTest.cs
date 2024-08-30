
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class GetRolesByApplicationIdSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        using var context = CreateContext();
        SeedData(i => new TypeMaster
        {
            Id = Guid.NewGuid(),
            Name = $"Type {i + 1}"
        }, 2);

        var typeMasters = context.TypeMasters.ToList();

        SeedData(i => new Role
        {
            Id = Guid.NewGuid(),
            ApplicationId = Guid.NewGuid(),
            TypeMasterId = typeMasters[i % typeMasters.Count].Id,
            RoleName = $"Role {i + 1}",
            IsActive = i % 2 == 0,
            IsDeleted = i % 3 == 0
        }, 10);        
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsRoles()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = context.Roles.First(role => role.IsActive && !role.IsDeleted).ApplicationId;
        var handler = new GetRolesByApplicationIdSqlHandler(context);
        var request = new GetRolesByApplicationId(applicationId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.All.Matches<Role>(role => role.ApplicationId == applicationId));
    }

    [Test]
    public async Task Handle_IncludeTypeMaster_ReturnsRolesWithTypeMaster()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = context.Roles.First(role => role.IsActive && !role.IsDeleted).ApplicationId;
        var handler = new GetRolesByApplicationIdSqlHandler(context);
        var request = new GetRolesByApplicationId(applicationId, includeTypeMaster: true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.All.Matches<Role>(role => role.ApplicationId == applicationId));
        Assert.That(result, Has.All.Matches<Role>(role => role.TypeMaster != null));
    }

    [Test]
    public async Task Handle_InactiveRoles_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = context.Roles.First(role => !role.IsActive && !role.IsDeleted).ApplicationId;
        var roles = context.Roles.Where(role => role.ApplicationId == applicationId);
        foreach (var role in roles)
        {
            role.IsActive = true;
        }
        await context.SaveChangesAsync();
        var handler = new GetRolesByApplicationIdSqlHandler(context);
        var request = new GetRolesByApplicationId(applicationId, isActive: false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_NonExistentApplication_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var nonExistentApplicationId = Guid.NewGuid();
        var handler = new GetRolesByApplicationIdSqlHandler(context);
        var request = new GetRolesByApplicationId(nonExistentApplicationId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

}
