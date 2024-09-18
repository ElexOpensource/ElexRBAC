using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

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
            Name = "New Application",
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
        Assert.That(result.Name, Is.EqualTo("New Application"));
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
            Name = "Existing Application",
            CustomerId = Guid.NewGuid(),
            IsActive = true,
            IsDeleted = false
        };
        context.Applications.Add(existingApplication);
        await context.SaveChangesAsync();

        existingApplication.Name = "Updated Application";

        var handler = new AddorUpdateApplicationsSqlHandler(context);
        var request = new AddorUpdateApplication(existingApplication);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingApplicationId));
        Assert.That(result.Name, Is.EqualTo("Updated Application"));
    }
}
