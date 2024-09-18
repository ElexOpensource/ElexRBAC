using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class DeleteAccessSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_ValidRequest_DeletesAccess()
    {
        // Arrange
        using var context = CreateContext();
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId, Name = "Test Access", IsActive = true, IsDeleted = false };
        context.Accesses.Add(access);
        await context.SaveChangesAsync();

        var handler = new DeleteAccessSqlHandler(context);
        var request = new DeleteAccess(accessId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);

        var deletedAccess = await context.Accesses.FindAsync(accessId);
        Assert.That(deletedAccess, Is.Null);
    }

    [Test]
    public async Task Handle_NonExistentAccess_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new DeleteAccessSqlHandler(context);
        var request = new DeleteAccess(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }
}
