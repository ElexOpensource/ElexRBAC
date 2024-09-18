using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class DeleteRoleSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_ValidRequest_DeletesRole()
    {
        // Arrange
        using var context = CreateContext();
        var existingRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Role",
            IsActive = true,
            IsDeleted = false
        };
        context.Roles.Add(existingRole);
        await context.SaveChangesAsync();

        var handler = new DeleteRoleSqlHandler(context);
        var request = new DeleteRole(existingRole.Id);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        var roleInDb = await context.Roles.FindAsync(existingRole.Id);
        Assert.That(roleInDb, Is.Null);
    }


    [Test]
    public async Task Handle_InvalidRequest_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new DeleteRoleSqlHandler(context);
        var request = new DeleteRole(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }
}
