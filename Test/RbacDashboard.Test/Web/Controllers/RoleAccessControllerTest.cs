

using Microsoft.AspNetCore.Mvc;
using Moq;
using Rbac.Controllers;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Models.Domain;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Test.Web;

public class RoleAccessControllerTest
{
    private Mock<IRbacRoleAccessRepository> _roleAccessRepositoryMock;
    private RoleAccessController _roleAccessController;

    [SetUp]
    public void SetUp()
    {
        // Create mocks for the repository
        _roleAccessRepositoryMock = new Mock<IRbacRoleAccessRepository>();

        // Create an instance of the RoleAccessController with the mocked repository
        _roleAccessController = new RoleAccessController(_roleAccessRepositoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _roleAccessController.Dispose();
    }

    [Test]
    public async Task GetByRoleId_ShouldReturnRoleAccess_WhenValidRoleIdProvided()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var expectedRoleAccessList = new List<RoleAccess>
        {
            new RoleAccess { Id = Guid.NewGuid(), RoleId = roleId, Access = new Access()},
            new RoleAccess { Id = Guid.NewGuid(), RoleId = roleId, Access = new Access() }
        };

        _roleAccessRepositoryMock
            .Setup(repo => repo.GetByRoleId(roleId))
            .ReturnsAsync(expectedRoleAccessList);

        // Act
        var result = await _roleAccessController.GetByRoleId(roleId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedRoleAccessList));
    }

    [Test]
    public void GetByRoleId_ShouldThrowError_WhenValidRoleIdEmpty()
    {
        // Arrange
        var roleId = Guid.Empty;

        // Act && Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _roleAccessController.GetByRoleId(roleId));
    }


    [Test]
    public async Task AddRemoveAccess_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var roleAccessRequest = new AddRemoveAccessRequest
        {
            RoleId = Guid.NewGuid(),
            AddAccess = new List<Guid> { Guid.NewGuid() },
            RemoveAccess = new List<Guid> { Guid.NewGuid() }
        };

        _roleAccessRepositoryMock
            .Setup(repo => repo.AddRemoveAccess(applicationId, roleAccessRequest))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roleAccessController.AddRemoveAccess(applicationId, roleAccessRequest);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    public async Task AddorUpdate_ShouldReturnRoleAccess_WhenRoleAccessIsAddedOrUpdated()
    {
        // Arrange
        var roleAccess = new RoleAccess
        {
            Id = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Access = new Access()
        };

        _roleAccessRepositoryMock
            .Setup(repo => repo.AddorUpdate(roleAccess))
            .ReturnsAsync(roleAccess);

        // Act
        var result = await _roleAccessController.AddorUpdate(roleAccess);

        // Assert
        Assert.That(result, Is.EqualTo(roleAccess));
    }

    [Test]
    public async Task Delete_ShouldReturnOk_WhenValidRoleAccessIdProvided()
    {
        // Arrange
        var roleAccessId = Guid.NewGuid();

        _roleAccessRepositoryMock
            .Setup(repo => repo.Delete(roleAccessId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roleAccessController.Delete(roleAccessId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    public void Delete_ThrowError_WhenRoleAccessIdEmpty()
    {
        // Arrange
        var roleId = Guid.Empty;

        // Act && Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _roleAccessController.Delete(roleId));
    }
}
