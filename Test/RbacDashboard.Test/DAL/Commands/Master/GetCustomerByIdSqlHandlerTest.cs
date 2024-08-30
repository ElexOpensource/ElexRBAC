
using Castle.Core.Resource;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Test;

public class GetCustomerByIdSqlHandlerTest : TestBase
{
    private readonly Guid _customerId1 = Guid.NewGuid();
    private readonly Guid _customerId2 = Guid.NewGuid();
    private readonly Guid _customerId3 = Guid.NewGuid();

    [SetUp]
    public void TestSetup()
    {
        var customer1 = new Customer { Id = _customerId1, CustomerName = "Customer One", IsActive = true, IsDeleted = false };
        var customer2 = new Customer { Id = _customerId2, CustomerName = "Customer Two", IsActive = true, IsDeleted = true };
        var customer3 = new Customer { Id = _customerId3, CustomerName = "Customer Three", IsActive = false, IsDeleted = false };
        SeedData([customer1, customer2, customer3]);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsCustomer()
    {
        // Arrange
        using var context = CreateContext();

        var handler = new GetCustomerByIdSqlHandler(context);
        var request = new GetCustomerById(_customerId1);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_customerId1));
    }

    [Test]
    public async Task Handle_InactiveCustomer_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetCustomerByIdSqlHandler(context);
        var request = new GetCustomerById(_customerId3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_DeletedCustomer_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();

        var handler = new GetCustomerByIdSqlHandler(context);
        var request = new GetCustomerById(_customerId2);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_CustomerNotFound_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetCustomerByIdSqlHandler(context);
        var request = new GetCustomerById(Guid.NewGuid());

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_InactiveCustomerWithIsActiveFalse_ReturnsCustomer()
    {
        // Arrange
        using var context = CreateContext();

        var handler = new GetCustomerByIdSqlHandler(context);
        var request = new GetCustomerById(_customerId3, false);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_customerId3));
    }
}
