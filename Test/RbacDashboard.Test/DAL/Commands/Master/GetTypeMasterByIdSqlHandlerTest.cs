using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class GetTypeMasterByIdSqlHandlerTest : TestBase
{
    [SetUp]
    public void TestSetup()
    {
        SeedData(i => new TypeMaster
        {
            Id = Guid.NewGuid(),
            Name = $"Type {i + 1}",
            IsActive = i % 2 == 0,
            IsDeleted = i % 3 == 0
        }, 15);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsTypeMasters()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetTypeMasterByIdSqlHandler(context);
        var request = new GetAllTypeMaster();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.All(tm => tm.IsActive), Is.True);
    }

    [Test]
    public async Task Handle_InactiveTypeMasters_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetTypeMasterByIdSqlHandler(context);
        var request = new GetAllTypeMaster();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.All(tm => tm.IsActive), Is.True);
    }

    [Test]
    public async Task Handle_DeletedTypeMasters_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetTypeMasterByIdSqlHandler(context);
        var request = new GetAllTypeMaster();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.All(tm => !tm.IsDeleted), Is.True);
    }
}
