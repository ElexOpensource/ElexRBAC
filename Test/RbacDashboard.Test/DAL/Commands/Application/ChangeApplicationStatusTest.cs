using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class ChangeApplicationStatusTest : TestBase
{
    private ChangeApplicationStatusSqlHandler _handler;

    [SetUp]
    public void Init()
    {
        var dbContext = CreateContext();
        _handler = new ChangeApplicationStatusSqlHandler(dbContext);
    }

    [Test]
    public async Task Handle_WhenApplicationNotFound_ReturnsFalse()
    {
        // Arrange
        var request = new ChangeApplicationStatus(Guid.NewGuid(), RecordStatus.Active);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Handle_WhenApplicationFoundAndStatusActive_SetsIsActiveTrue()
    {
        // Arrange
        var application = new Application { Id = Guid.NewGuid(), Name = "Application", IsActive = false };
        SeedData(new List<Application> { application });
        var request = new ChangeApplicationStatus(application.Id, RecordStatus.Active);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedApplication = await context.Applications.FindAsync(application.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedApplication.IsActive, Is.True);
    }

    [Test]
    public async Task Handle_WhenApplicationFoundAndStatusInactive_SetsIsActiveFalse()
    {
        // Arrange
        var application = new Application { Id = Guid.NewGuid(), Name = "Application", IsActive = true };
        SeedData(new List<Application> { application });
        var request = new ChangeApplicationStatus(application.Id, RecordStatus.Inactive);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedApplication = await context.Applications.FindAsync(application.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedApplication.IsActive, Is.False);
    }

    [Test]
    public async Task Handle_WhenApplicationFoundAndStatusDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var application = new Application { Id = Guid.NewGuid(), Name = "Application", IsDeleted = false };
        SeedData(new List<Application> { application });
        var request = new ChangeApplicationStatus(application.Id, RecordStatus.Delete);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedApplication = await context.Applications.FindAsync(application.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedApplication.IsDeleted, Is.True);
    }
}
