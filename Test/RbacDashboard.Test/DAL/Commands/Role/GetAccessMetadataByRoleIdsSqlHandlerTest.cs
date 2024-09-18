using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class GetAccessMetadataByRoleIdsSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {

        using var context = CreateContext();
        SeedData(i => new Role
        {
            Id = Guid.NewGuid(),
            Name = $"Role {i + 1}",
            IsActive = true,
            IsDeleted = false
        }, 3);

        SeedData(i => new Access
        {
            Id = Guid.NewGuid(),
            Name = $"Access {i + 1}",
            IsActive = true,
            IsDeleted = false
        }, 3);

        // Seed role accesses with valid role and access IDs
        var roles = context.Roles.ToList();
        var accesses = context.Accesses.ToList();
        SeedData(i => new RoleAccess
        {
            RoleId = roles[i % roles.Count].Id,
            AccessId = accesses[i % accesses.Count].Id,
            IsActive = true,
            IsDeleted = false
        }, 6);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsRoleAccessList()
    {
        // Arrange
        using var context = CreateContext();
        var roleIds = context.Roles.Select(r => r.Id).ToList();

        var handler = new GetAccessMetadataByRoleIdsSqlHandler(context);
        var request = new GetRoleAccessByRoleIds(roleIds);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(6));
    }

    [Test]
    public async Task Handle_NoMatchingRoleIds_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var roleId = Guid.NewGuid();

        var handler = new GetAccessMetadataByRoleIdsSqlHandler(context);
        var request = new GetRoleAccessByRoleIds(new List<Guid> { roleId });

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_RoleOrAccessNotActiveOrDeleted_ReturnsFilteredRoleAccessList()
    {
        // Arrange
        using var context = CreateContext();
        var role = context.Roles.First();
        role.IsActive = false;
        context.Roles.Update(role);
        await context.SaveChangesAsync();

        var handler = new GetAccessMetadataByRoleIdsSqlHandler(context);
        var request = new GetRoleAccessByRoleIds(new List<Guid> { role.Id });

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
