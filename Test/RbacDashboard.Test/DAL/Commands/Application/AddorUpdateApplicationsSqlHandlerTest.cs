using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class AddorUpdateApplicationsSqlHandlerTest : TestBase
{
    [Test]
    public async Task Handle_AddNewApplication_AddsApplication()
    {
        // Arrange
        using var context = CreateContext();
        var newApplication = new Application
        {
            Id = Guid.Empty,
            ApplicationName = "New Application",
            CustomerId = Guid.NewGuid(),
            IsActive = true,
            IsDeleted = false
        };

        var handler = new AddorUpdateApplicationsSqlHandler(context);
        var request = new AddorUpdateApplication(newApplication);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.ApplicationName, Is.EqualTo("New Application"));
    }

    [Test]
    public async Task Handle_UpdateExistingApplication_UpdatesApplication()
    {
        // Arrange
        using var context = CreateContext();
        var existingApplicationId = Guid.NewGuid();
        var existingApplication = new Application
        {
            Id = existingApplicationId,
            ApplicationName = "Existing Application",
            CustomerId = Guid.NewGuid(),
            IsActive = true,
            IsDeleted = false
        };
        context.Applications.Add(existingApplication);
        await context.SaveChangesAsync();

        existingApplication.ApplicationName = "Updated Application";

        var handler = new AddorUpdateApplicationsSqlHandler(context);
        var request = new AddorUpdateApplication(existingApplication);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingApplicationId));
        Assert.That(result.ApplicationName, Is.EqualTo("Updated Application"));
    }
}
