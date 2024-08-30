
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class DeleteApplicationSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_ValidRequest_DeletesApplication()
    {
        // Arrange
        using var context = CreateContext();
        var applicationId = Guid.NewGuid();
        var application = new Application { Id = applicationId, ApplicationName = "Test Application", CustomerId = Guid.NewGuid(), IsActive = true, IsDeleted = false };
        context.Applications.Add(application);
        await context.SaveChangesAsync();

        var handler = new DeleteApplicationSqlHandler(context);
        var request = new DeleteApplication(applicationId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);

        var deletedApplication = await context.Applications.FindAsync(applicationId);
        Assert.That(deletedApplication, Is.Null);

        var exists = await context.Applications.AnyAsync(a => a.Id == applicationId);
        Assert.That(exists, Is.False);
    }

    [Test]
    public async Task Handle_NonExistentApplication_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new DeleteApplicationSqlHandler(context);
        var request = new DeleteApplication(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }
}
