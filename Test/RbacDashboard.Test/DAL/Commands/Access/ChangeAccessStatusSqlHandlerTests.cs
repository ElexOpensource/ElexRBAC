using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class ChangeAccessStatusSqlHandlerTests : TestBase
{
    private ChangeAccessStatusSqlHandler _handler;

    [SetUp]
    public void Init()
    {
        var dbContext = CreateContext();
        _handler = new ChangeAccessStatusSqlHandler(dbContext);
    }

    [Test]
    public async Task Handle_WhenAccessNotFound_ReturnsFalse()
    {
        // Arrange
        var request = new ChangeAccessStatus(Guid.NewGuid(), RecordStatus.Active);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Handle_WhenAccessFoundAndStatusActive_SetsIsActiveTrue()
    {
        // Arrange
        var access = new Access { Id = Guid.NewGuid(), Name = "Access", IsActive = false };
        SeedData(new List<Access> { access });
        var request = new ChangeAccessStatus(access.Id, RecordStatus.Active);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedAccess = await context.Accesses.FindAsync(access.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedAccess.IsActive, Is.True);
    }

    [Test]
    public async Task Handle_WhenAccessFoundAndStatusInactive_SetsIsActiveFalse()
    {
        // Arrange
        var access = new Access { Id = Guid.NewGuid(), Name = "Access", IsActive = true };
        SeedData(new List<Access> { access });
        var request = new ChangeAccessStatus(access.Id, RecordStatus.Inactive);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedAccess = await context.Accesses.FindAsync(access.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedAccess.IsActive, Is.False);
    }

    [Test]
    public async Task Handle_WhenAccessFoundAndStatusDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var access = new Access { Id = Guid.NewGuid(), Name = "Access", IsDeleted = false };
        SeedData(new List<Access> { access });
        var request = new ChangeAccessStatus(access.Id, RecordStatus.Delete);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        using var context = CreateContext();
        var updatedAccess = await context.Accesses.FindAsync(access.Id);
        Assert.That(result, Is.True);
        Assert.That(updatedAccess.IsDeleted, Is.True);
    }
}
