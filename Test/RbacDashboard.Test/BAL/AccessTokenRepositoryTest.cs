
using Moq;
using RbacDashboard.Common;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.BAL.Test;

public class AccessTokenRepositoryTest
{

    private Mock<IMediatorService> _mediatorMock;
    private AccessRepository _accessRepository;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediatorService>();
        _accessRepository = new AccessRepository(_mediatorMock.Object);
    }

    [Test]
    public async Task AddorUpdate_ShouldCallMediatorServiceWithCorrectRequest()
    {
        // Arrange
        var access = new Access { Id = Guid.NewGuid(), AccessName = "TestAccess" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<AddorUpdateAccess>()))
            .ReturnsAsync(access);

        // Act
        var result = await _accessRepository.AddorUpdate(access);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<AddorUpdateAccess>(req => req.Access == access)), Times.Once);
        Assert.That(result, Is.EqualTo(access));
    }

    [Test]
    public async Task Delete_ShouldCallMediatorServiceWithCorrectRequest()
    {
        // Arrange
        var accessId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<DeleteAccess>()))
            .ReturnsAsync(true);

        // Act
        await _accessRepository.Delete(accessId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<DeleteAccess>(req => req.Id == accessId)), Times.Once);
    }

    [Test]
    public async Task GetById_ShouldReturnAccess_WhenAccessExists()
    {
        // Arrange
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId, AccessName = "TestAccess" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetAccessById>()))
            .ReturnsAsync(access);

        // Act
        var result = await _accessRepository.GetById(accessId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetAccessById>(req => req.Id == accessId)), Times.Once);
        Assert.That(result, Is.EqualTo(access));
    }

    [Test]
    public void GetById_ShouldThrowKeyNotFoundException_WhenAccessDoesNotExist()
    {
        // Arrange
        var accessId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetAccessById>()))
                     .ReturnsAsync((Access)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _accessRepository.GetById(accessId));
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetAccessById>(req => req.Id == accessId)), Times.Once);
    }

    [Test]
    public async Task GetByApplicationId_ShouldReturnListOfAccesses()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var accesses = new List<Access>
            {
                new Access { Id = Guid.NewGuid(), AccessName = "Access1" },
                new Access { Id = Guid.NewGuid(), AccessName = "Access2" }
            };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetAccessesByApplicationId>()))
            .ReturnsAsync(accesses);

        // Act
        var result = await _accessRepository.GetByApplicationId(applicationId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetAccessesByApplicationId>(req => req.ApplicationId == applicationId)), Times.Once);
        Assert.That(result, Is.EqualTo(accesses));
    }
}