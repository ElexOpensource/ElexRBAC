using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class GetOptionsetMastersSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        SeedData(i => new OptionsetMaster
        {
            Id = Guid.NewGuid(),
            Name = $"Optionset {i + 1}",
            JsonObject = string.Empty ,
            IsActive = i % 2 == 0,
            IsDeleted = i % 3 == 0
        }, 15);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsOptionsetMasters()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetOptionsetMastersSqlHandler(context);
        var request = new GetOptionsetMasters();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_ValidRequestWithIsActiveFalse_ReturnsInactiveOptionsetMasters()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetOptionsetMastersSqlHandler(context);
        var request = new GetOptionsetMasters(isActive: false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_NoOptionsetMasters_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        context.OptionsetMasters.RemoveRange(context.OptionsetMasters);
        await context.SaveChangesAsync();

        var handler = new GetOptionsetMastersSqlHandler(context);
        var request = new GetOptionsetMasters();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
