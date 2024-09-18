

using Microsoft.AspNetCore.Mvc;
using Moq;
using Rbac.Controllers;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Test.Web;

public class AccessControllerTest
{
    private Mock<IRbacAccessRepository> _accessRepositoryMock;
    private AccessController _controller;

    [SetUp]
    public void Setup()
    {
        _accessRepositoryMock = new Mock<IRbacAccessRepository>();
        _controller = new AccessController(_accessRepositoryMock.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task GetById_ShouldReturnAccess_WhenIdIsValid()
    {
        // Arrange
        var accessId = Guid.NewGuid();
        var access = new Access { Id = accessId };

        _accessRepositoryMock.Setup(repo => repo.GetById(accessId)).ReturnsAsync(access);

        // Act
        var result = await _controller.GetById(accessId);

        // Assert
        Assert.That(result, Is.EqualTo(access));
        _accessRepositoryMock.Verify(repo => repo.GetById(accessId), Times.Once);
    }

    [Test]
    public void GetById_ShouldThrowArgumentNullException_WhenIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.GetById(Guid.Empty));
        _accessRepositoryMock.Verify(repo => repo.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task GetByApplicationId_ShouldReturnAccessList_WhenApplicationIdIsValid()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var accessList = new List<Access> { new Access { ApplicationId = applicationId } };

        _accessRepositoryMock.Setup(repo => repo.GetByApplicationId(applicationId, true)).ReturnsAsync(accessList);

        // Act
        var result = await _controller.GetByApplicationId(applicationId);

        // Assert
        Assert.That(result, Is.EqualTo(accessList));
        _accessRepositoryMock.Verify(repo => repo.GetByApplicationId(applicationId, true), Times.Once);
    }

    [Test]
    public void GetByApplicationId_ShouldThrowArgumentNullException_WhenApplicationIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.GetByApplicationId(Guid.Empty));
        _accessRepositoryMock.Verify(repo => repo.GetByApplicationId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public async Task AddorUpdate_ShouldReturnUpdatedAccess_WhenAccessIsValid()
    {
        // Arrange
        var access = new Access { Id = Guid.NewGuid() };

        _accessRepositoryMock.Setup(repo => repo.AddorUpdate(access)).ReturnsAsync(access);

        // Act
        var result = await _controller.AddorUpdate(access);

        // Assert
        Assert.That(result, Is.EqualTo(access));
        _accessRepositoryMock.Verify(repo => repo.AddorUpdate(access), Times.Once);
    }

    [Test]
    public async Task Delete_ShouldReturnOkResult_WhenAccessIdIsValid()
    {
        // Arrange
        var accessId = Guid.NewGuid();

        _accessRepositoryMock.Setup(repo => repo.Delete(accessId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(accessId);

        // Assert
        Assert.That(result, Is.TypeOf<OkResult>());
        _accessRepositoryMock.Verify(repo => repo.Delete(accessId), Times.Once);
    }

    [Test]
    public void Delete_ShouldThrowArgumentNullException_WhenAccessIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.Delete(Guid.Empty));
        _accessRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task ChangeStatus_ShouldReturnOkResult_WhenAccessIdIsValid()
    {
        // Arrange
        var accessId = Guid.NewGuid();

        _accessRepositoryMock.Setup(repo => repo.ChangeStatus(accessId, RecordStatus.Active)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ChangeStatus(accessId, RecordStatus.Active);

        // Assert
        Assert.That(result, Is.TypeOf<OkResult>());
        _accessRepositoryMock.Verify(repo => repo.ChangeStatus(accessId, RecordStatus.Active), Times.Once);
    }

    [Test]
    public void ChangeStatus_ShouldThrowArgumentNullException_WhenAccessIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.ChangeStatus(Guid.Empty, RecordStatus.Active));
        _accessRepositoryMock.Verify(repo => repo.ChangeStatus(It.IsAny<Guid>(), RecordStatus.Active), Times.Never);
    }
}
