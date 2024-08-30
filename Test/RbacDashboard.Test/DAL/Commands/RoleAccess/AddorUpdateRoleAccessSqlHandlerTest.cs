
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class AddorUpdateRoleAccessSqlHandlerTest : TestBase
{
    private readonly Guid _roleId = Guid.NewGuid();
    private readonly Guid _AccessId = Guid.NewGuid();

    [SetUp]
    public void TestSetup()
    {
        using var context = CreateContext();
        SeedData(i => new TypeMaster
        {
            Id = Guid.NewGuid(),
            Name = $"Type {i + 1}"
        }, 1);

        var typeMasters = context.TypeMasters.ToList();

        SeedData(i => new Role
        {
            Id = _roleId,
            ApplicationId = Guid.NewGuid(),
            TypeMasterId = typeMasters[i % typeMasters.Count].Id,
            RoleName = $"Role {i + 1}",
            IsActive = true,
            IsDeleted = false
        }, 1);

        SeedData(i => new Access
        {
            Id = _AccessId,
            ApplicationId = Guid.NewGuid(),
            AccessName = $"Access {i + 1}",
            MetaData = string.Empty,
            IsActive = true,
            IsDeleted = false
        }, 1);

        SeedData(i => new RoleAccess
        {
            Id = Guid.NewGuid(),
            RoleId = _roleId,
            AccessId = _AccessId,
            IsActive = true,
            IsDeleted = false
        }, 1);
    }

    [Test]
    public async Task Handle_AddNewRoleAccess_ReturnsAddedRoleAccess()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new AddorUpdateRoleAccessSqlHandler(context);

        var newRoleAccess = new RoleAccess
        {
            Id = Guid.Empty,
            RoleId = _roleId,
            AccessId = _AccessId,
            IsActive = true,
            IsDeleted = false
        };

        var request = new AddorUpdateRoleAccess(newRoleAccess);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(context.RoleAccesses, Does.Contain(result));
    }

    [Test]
    public async Task Handle_UpdateExistingRoleAccess_ReturnsUpdatedRoleAccess()
    {
        // Arrange
        using var context = CreateContext();
        var existingRoleAccess = context.RoleAccesses.First();
        existingRoleAccess.IsActive = false;

        var handler = new AddorUpdateRoleAccessSqlHandler(context);
        var request = new AddorUpdateRoleAccess(existingRoleAccess);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingRoleAccess.Id));
        Assert.That(result.IsActive, Is.False);
    }
}
