using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class GetPermissionsetsSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        SeedData(i => new Permissionset
        {
            Id = Guid.NewGuid(),
            Name = $"Permissionset {i + 1}",
            IsActive = i % 2 == 0,
            IsDeleted = i % 3 == 0
        }, 15);
    }


    [Test]
    public async Task Handle_ValidRequest_ReturnsPermissionsets()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetPermissionsetsSqlHandler(context);
        var request = new GetPermissionsets();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_ValidRequestWithIsActiveFalse_ReturnsInactivePermissionsets()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetPermissionsetsSqlHandler(context);
        var request = new GetPermissionsets(isActive: false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_NoPermissionsets_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        context.Permissionsets.RemoveRange(context.Permissionsets);
        await context.SaveChangesAsync();

        var handler = new GetPermissionsetsSqlHandler(context);
        var request = new GetPermissionsets();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
