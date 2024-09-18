using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class GetApplicationByIdSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        SeedData(i => new Application
        {
            Id = Guid.NewGuid(),
            Name = $"App {i + 1}",
            CustomerId = Guid.NewGuid(),
            IsActive = true,
            IsDeleted = false
        }, 10);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsApplication()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetApplicationByIdSqlHandler(context);
        var applicationId = context.Applications.First().Id;
        var request = new GetApplicationById(applicationId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(applicationId));
    }

    [Test]
    public async Task Handle_InvalidRequest_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetApplicationByIdSqlHandler(context);
        var request = new GetApplicationById(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}
