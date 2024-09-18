
using Microsoft.AspNetCore.Mvc;
using Moq;
using Rbac.Controllers;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Test.Web;

public class ApplicationControllerTest
{
    private Mock<IRbacApplicationRepository> _applicationRepositoryMock;
    private ApplicationController _controller;

    [SetUp]
    public void SetUp()
    {
        _applicationRepositoryMock = new Mock<IRbacApplicationRepository>();
        _controller = new ApplicationController(_applicationRepositoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task GetById_ShouldReturnApplication_WhenValidIdProvided()
    {
        // Arrange
        var validApplicationId = Guid.NewGuid();
        var expectedApplication = new Application { Id = validApplicationId, Name = "Test Application" };

        _applicationRepositoryMock
            .Setup(repo => repo.GetById(validApplicationId))
            .ReturnsAsync(expectedApplication);

        // Act
        var result = await _controller.GetById(validApplicationId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedApplication));
    }

    [Test]
    public void GetById_ShouldThrowArgumentNullException_WhenEmptyIdProvided()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.GetById(emptyGuid));
    }

    [Test]
    public async Task GetByCustomerId_ShouldReturnApplications_WhenValidCustomerIdProvided()
    {
        // Arrange
        var validCustomerId = Guid.NewGuid();
        var expectedApplications = new List<Application>
        {
            new Application { Id = Guid.NewGuid(), Name = "App 1" },
            new Application { Id = Guid.NewGuid(), Name = "App 2" }
        };

        _applicationRepositoryMock
            .Setup(repo => repo.GetByCustomerId(validCustomerId, true))
            .ReturnsAsync(expectedApplications);

        // Act
        var result = await _controller.GetByCustomerId(validCustomerId, true);

        // Assert
        Assert.That(result, Is.EqualTo(expectedApplications));
    }

    [Test]
    public void GetByCustomerId_ShouldThrowArgumentNullException_WhenEmptyIdProvided()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.GetByCustomerId(emptyGuid, true));
    }

    [Test]
    public async Task AddorUpdate_ShouldReturnApplication_WhenValidApplicationProvided()
    {
        // Arrange
        var newApplication = new Application { Name = "New Application" };
        var expectedApplication = new Application { Id = Guid.NewGuid(), Name = "New Application" };

        _applicationRepositoryMock
            .Setup(repo => repo.AddorUpdate(newApplication))
            .ReturnsAsync(expectedApplication);

        // Act
        var result = await _controller.AddorUpdate(newApplication);

        // Assert
        Assert.That(result, Is.EqualTo(expectedApplication));
    }

    [Test]
    public async Task Delete_ShouldReturnOk_WhenValidIdProvided()
    {
        // Arrange
        var validApplicationId = Guid.NewGuid();

        _applicationRepositoryMock
            .Setup(repo => repo.Delete(validApplicationId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(validApplicationId);

        // Assert
        Assert.That(result, Is.TypeOf<OkResult>());
    }

    [Test]
    public void Delete_ShouldThrowArgumentNullException_WhenEmptyIdProvided()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.Delete(emptyGuid));
    }

    [Test]
    public async Task ChangeStatus_ShouldReturnOk_WhenValidIdProvided()
    {
        // Arrange
        var validApplicationId = Guid.NewGuid();

        _applicationRepositoryMock
            .Setup(repo => repo.ChangeStatus(validApplicationId, RecordStatus.Active))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ChangeStatus(validApplicationId, RecordStatus.Active);

        // Assert
        Assert.That(result, Is.TypeOf<OkResult>());
    }

    [Test]
    public void ChangeStatus_ShouldThrowArgumentNullException_WhenEmptyIdProvided()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _controller.ChangeStatus(emptyGuid, RecordStatus.Active));
    }
}
