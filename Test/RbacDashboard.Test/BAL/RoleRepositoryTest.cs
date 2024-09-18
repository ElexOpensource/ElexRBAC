

using Moq;
using RbacDashboard.Common;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.BAL.Test;

public class RoleRepositoryTest
{
    private Mock<IMediatorService> _mediatorMock;
    private RoleRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediatorService>();
        _repository = new RoleRepository(_mediatorMock.Object);
    }

    [Test]
    public async Task AddorUpdate_ShouldReturnUpdatedRole_WhenCalled()
    {
        // Arrange
        var role = new Role { Id = Guid.NewGuid(), RoleName = "Test Role" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<AddorUpdateRole>()))
                     .ReturnsAsync(role);

        // Act
        var result = await _repository.AddorUpdate(role);

        // Assert
        Assert.That(result, Is.EqualTo(role));
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<AddorUpdateRole>()), Times.Once);
    }

    [Test]
    public async Task Delete_ShouldCallSendRequest_WhenCalled()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        await _repository.Delete(roleId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<DeleteRole>(request => request.Id == roleId)), Times.Once);
    }

    [Test]
    public void GetById_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var role = new Role { Id = Guid.NewGuid(), RoleName = "Test Role" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleById>()))
                     .ReturnsAsync(role);

        // Act
        var result = _repository.GetById(role.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.EqualTo(role));
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<GetRoleById>()), Times.Once);
    }

    [Test]
    public void GetById_ShouldThrowKeyNotFoundException_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleById>()))
                     .ReturnsAsync((Role)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetById(roleId));
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<GetRoleById>()), Times.Once);
    }

    [Test]
    public async Task GetByApplicationId_ShouldReturnRoles_WhenCalled()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var roles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), RoleName = "Role 1" },
            new Role { Id = Guid.NewGuid(), RoleName = "Role 2" }
        };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
                     .ReturnsAsync(roles);

        // Act
        var result = await _repository.GetByApplicationId(applicationId, true);

        // Assert
        Assert.That(result, Is.EqualTo(roles));
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()), Times.Once);
    }
}
