
using Moq;
using RbacDashboard.BAL;
using RbacDashboard.Common;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Test.BAL;

public class ApplicationRepositoryTest
{
    private Mock<IMediatorService> _mediatorMock;
    private ApplicationRepository _applicationRepository;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediatorService>();
        _applicationRepository = new ApplicationRepository(_mediatorMock.Object);
    }

    [Test]
    public async Task AddorUpdate_ShouldReturnApplication_WhenCalled()
    {
        // Arrange
        var application = new Application { Id = Guid.NewGuid(), Name = "TestApplication" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<AddorUpdateApplication>()))
            .ReturnsAsync(application);

        // Act
        var result = await _applicationRepository.AddorUpdate(application);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<AddorUpdateApplication>(req => req.Application == application)), Times.Once);
        Assert.That(result, Is.EqualTo(application));
    }

    [Test]
    public async Task Delete_ShouldCallMediatorSendRequest_WhenCalled()
    {
        // Arrange
        var applicationId = Guid.NewGuid();

        // Act
        await _applicationRepository.Delete(applicationId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<DeleteApplication>(req => req.Id == applicationId)), Times.Once);
    }

    [Test]
    public void GetById_ShouldThrowKeyNotFoundException_WhenApplicationNotFound()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetApplicationById>()))
            .ReturnsAsync((Application)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _applicationRepository.GetById(applicationId));
        Assert.That(ex.Message, Is.EqualTo($"Application with id - {applicationId} is not available"));
    }

    [Test]
    public async Task GetById_ShouldReturnApplication_WhenApplicationFound()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var application = new Application { Id = applicationId, Name = "TestApplication" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetApplicationById>()))
            .ReturnsAsync(application);

        // Act
        var result = await _applicationRepository.GetById(applicationId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetApplicationById>(req => req.Id == applicationId)), Times.Once);
        Assert.That(result, Is.EqualTo(application));
    }

    [Test]
    public async Task GetByCustomerId_ShouldReturnApplications_WhenCalled()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var applications = new List<Application>
            {
                new Application { Id = Guid.NewGuid(), Name = "TestApplication1" },
                new Application { Id = Guid.NewGuid(), Name = "TestApplication2" }
            };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetApplicationByCustomerId>()))
            .ReturnsAsync(applications);

        // Act
        var result = await _applicationRepository.GetByCustomerId(customerId, true);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetApplicationByCustomerId>(req => req.CustomerId == customerId)), Times.Once);
        Assert.That(result, Is.EqualTo(applications));
    }
}
