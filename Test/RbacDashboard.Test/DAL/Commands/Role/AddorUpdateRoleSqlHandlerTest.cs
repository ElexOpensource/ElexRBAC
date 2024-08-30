using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class AddorUpdateRoleSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_AddNewRole_ReturnsAddedRole()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new AddorUpdateRoleSqlHandler(context);
        var role = new Role
        {
            RoleName = "New Role",
            IsActive = true,
            IsDeleted = false
        };
        var request = new AddorUpdateRole(role);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.RoleName, Is.EqualTo("New Role"));
    }

    [Test]
    public async Task Handle_UpdateExistingRole_ReturnsUpdatedRole()
    {
        // Arrange
        using var context = CreateContext();
        var existingRole = new Role
        {
            Id = Guid.NewGuid(),
            RoleName = "Existing Role",
            IsActive = true,
            IsDeleted = false
        };
        context.Roles.Add(existingRole);
        await context.SaveChangesAsync();
        context.Entry(existingRole).State = EntityState.Detached;

        var handler = new AddorUpdateRoleSqlHandler(context);
        var updatedRole = new Role
        {
            Id = existingRole.Id,
            RoleName = "Updated Role",
            IsActive = false,
            IsDeleted = false
        };
        var request = new AddorUpdateRole(updatedRole);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingRole.Id));
        Assert.That(result.RoleName, Is.EqualTo("Updated Role"));
        Assert.That(result.IsActive, Is.False);
    }

    [Test]
    public async Task Handle_EmptyRoleId_AddsNewRole()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new AddorUpdateRoleSqlHandler(context);
        var role = new Role
        {
            RoleName = "New Role Without Id",
            IsActive = true,
            IsDeleted = false
        };
        var request = new AddorUpdateRole(role);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.RoleName, Is.EqualTo("New Role Without Id"));
        Assert.That(await context.Roles.CountAsync(), Is.EqualTo(1));
    }
}
