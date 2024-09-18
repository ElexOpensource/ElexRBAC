using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.Test.DAL.Base;

namespace RbacDashboard.Test.DAL;

public class GetApplicationsByCustomerIdSqlHandlerTest : TestBase
{
    private Guid _customerId;

    [SetUp]
    public void TestSetup()
    {
        _customerId = Guid.NewGuid();
        var _anotherCustomerId = Guid.NewGuid();

        SeedData(i => new Customer
        {
            Id = i % 2 == 0 ? _customerId : _anotherCustomerId,
            Name = $"Customer {i + 1}",
            IsActive = i % 2 == 0,
            IsDeleted = false
        }, 2);

        SeedData(i => new Application
        {
            Id = Guid.NewGuid(),
            Name = $"Application {i + 1}",
            CustomerId = i % 2 == 0 ? _customerId : _anotherCustomerId,
            IsActive = i % 2 == 0,
            IsDeleted = false
        }, 10);
    }


    [Test]
    public async Task Handle_ValidRequest_ReturnsApplications()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetApplicationsByCustomerIdSqlHandler(context);
        var request = new GetApplicationByCustomerId(_customerId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_IncludesCustomer_ReturnsApplicationsWithCustomer()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetApplicationsByCustomerIdSqlHandler(context);
        var request = new GetApplicationByCustomerId(_customerId, true, true);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.All(app => app.Customer != null), Is.True);
    }

    [Test]
    public async Task Handle_NoActiveApplications_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetApplicationsByCustomerIdSqlHandler(context);
        var request = new GetApplicationByCustomerId(_customerId, false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_EmptyRequest_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetApplicationsByCustomerIdSqlHandler(context);
        var request = new GetApplicationByCustomerId(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }


}
