﻿
using Microsoft.AspNetCore.Mvc;
using Moq;
using Rbac.Controllers;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Test.Web;

public class RoleControllerTest
{
    private Mock<IRbacRoleRepository> _roleRepositoryMock;
    private RoleController _roleController;

    [SetUp]
    public void SetUp()
    {
        // Create mocks for the repository
        _roleRepositoryMock = new Mock<IRbacRoleRepository>();

        // Create an instance of the RoleController with the mocked repository
        _roleController = new RoleController(_roleRepositoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _roleController.Dispose();
    }

    [Test]
    public async Task GetById_ShouldReturnRole_WhenValidRoleIdProvided()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var expectedRole = new Role { Id = roleId, Name = "Admin" };

        _roleRepositoryMock
            .Setup(repo => repo.GetById(roleId))
            .ReturnsAsync(expectedRole);

        // Act
        var result = await _roleController.GetById(roleId) as Role;

        // Assert
        Assert.That(result, Is.EqualTo(expectedRole));
    }

    [Test]
    public async Task GetByApplicationId_ShouldReturnRoles_WhenValidApplicationIdProvided()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var expectedRoles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "Admin", ApplicationId = applicationId },
            new Role { Id = Guid.NewGuid(), Name = "User", ApplicationId = applicationId }
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetByApplicationId(applicationId, true))
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _roleController.GetByApplicationId(applicationId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedRoles));
    }

    [Test]
    public async Task GetAvailableParentsById_ShouldReturnParentRoles_WhenValidApplicationIdProvided()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var expectedRoles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "Admin", ApplicationId = applicationId },
            new Role { Id = Guid.NewGuid(), Name = "User", ApplicationId = applicationId }
        };

        _roleRepositoryMock
            .Setup(repo => repo.GetAvailableParentsById(applicationId, roleId))
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _roleController.GetAvailableParentsById(applicationId, roleId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedRoles));
    }

    [Test]
    public async Task AddorUpdate_ShouldReturnRole_WhenRoleIsAddedOrUpdated()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        _roleRepositoryMock
            .Setup(repo => repo.AddorUpdate(role))
            .ReturnsAsync(role);

        // Act
        var result = await _roleController.AddorUpdate(role);

        // Assert
        Assert.That(result, Is.EqualTo(role));
    }

    [Test]
    public async Task Delete_ShouldReturnOk_WhenValidRoleIdProvided()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleRepositoryMock
            .Setup(repo => repo.Delete(roleId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roleController.Delete(roleId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    public void GetById_ShouldThrowArgumentNullException_WhenRoleIdIsEmpty()
    {
        // Arrange
        var roleId = Guid.Empty;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _roleController.GetById(roleId));
        Assert.That(ex.ParamName, Is.EqualTo(nameof(roleId)));
    }

    [Test]
    public void GetByApplicationId_ShouldThrowArgumentNullException_WhenApplicationIdIsEmpty()
    {
        // Arrange
        var applicationId = Guid.Empty;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _roleController.GetByApplicationId(applicationId));
        Assert.That(ex.ParamName, Is.EqualTo(nameof(applicationId)));
    }

    [Test]
    public void GetAvailableParentsById_ShouldThrowArgumentNullException_WhenApplicationIdIsEmpty()
    {
        // Arrange
        var applicationId = Guid.Empty;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _roleController.GetAvailableParentsById(applicationId, Guid.NewGuid()));
        Assert.That(ex.ParamName, Is.EqualTo(nameof(applicationId)));
    }

    [Test]
    public void Delete_ShouldThrowArgumentNullException_WhenRoleIdIsEmpty()
    {
        // Arrange
        var roleId = Guid.Empty;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _roleController.Delete(roleId));
        Assert.That(ex.ParamName, Is.EqualTo(nameof(roleId)));
    }

    [Test]
    public async Task ChangeStatus_ShouldReturnOk_WhenValidRoleIdProvided()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleRepositoryMock
            .Setup(repo => repo.ChangeStatus(roleId, RecordStatus.Active))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roleController.ChangeStatus(roleId, RecordStatus.Active);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }


    [Test]
    public void ChangeStatus_ShouldThrowArgumentNullException_WhenRoleIdIsEmpty()
    {
        // Arrange
        var roleId = Guid.Empty;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _roleController.ChangeStatus(roleId, RecordStatus.Active));
        Assert.That(ex.ParamName, Is.EqualTo(nameof(roleId)));
    }
}
