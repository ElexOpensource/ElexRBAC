
using Moq;
using RbacDashboard.BAL;
using RbacDashboard.Common;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.Test.BAL;

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
        var role = new Role { Id = Guid.NewGuid(), Name = "Test Role" };
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
        var role = new Role { Id = Guid.NewGuid(), Name = "Test Role" };
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
            new Role { Id = Guid.NewGuid(), Name = "Role 1" },
            new Role { Id = Guid.NewGuid(), Name = "Role 2" }
        };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
                     .ReturnsAsync(roles);

        // Act
        var result = await _repository.GetByApplicationId(applicationId, true);

        // Assert
        Assert.That(result, Is.EqualTo(roles));
        _mediatorMock.Verify(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()), Times.Once);
    }

    [Test]
    public void ChangeStatus_WhenStatusChanged_ShouldNotThrowException()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var status = RecordStatus.Active;

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<ChangeRoleStatus>()))
            .ReturnsAsync(true);

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _repository.ChangeStatus(roleId, status));
    }

    [Test]
    public void ChangeStatus_WhenStatusNotChanged_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var status = RecordStatus.Active;

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<ChangeRoleStatus>()))
            .ReturnsAsync(false);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.ChangeStatus(roleId, status));
        Assert.That($"Role with id - {roleId} is not available", Is.EqualTo(ex.Message));
    }

    [Test]
    public async Task GetAvailableParentsById_WhenCurrentRoleIdIsEmpty_ShouldReturnAllRoles()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var currentRoleId = Guid.Empty; // No current role selected

        var roles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "Role 1" },
            new Role { Id = Guid.NewGuid(), Name = "Role 2" }
        };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _repository.GetAvailableParentsById(applicationId, currentRoleId);

        // Assert
        Assert.That(roles.Count, Is.EqualTo(result.Count));
        Assert.That(roles,Is.EqualTo(result));
    }

    [Test]
    public async Task GetAvailableParentsById_WhenCurrentRoleHasChildren_ShouldExcludeChildRoles()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var currentRoleId = Guid.NewGuid();

        var parentRole = new Role { Id = currentRoleId, Name = "Parent Role" };
        var childRole = new Role { Id = Guid.NewGuid(), Name = "Child Role", ParentId = currentRoleId };
        var unrelatedRole = new Role { Id = Guid.NewGuid(), Name = "Unrelated Role" };

        var roles = new List<Role> { parentRole, childRole, unrelatedRole };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _repository.GetAvailableParentsById(applicationId, currentRoleId);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1)); // Only unrelatedRole should be available
        Assert.That(result.Any(r => r.Id == currentRoleId), Is.False); // Exclude current role
        Assert.That(result.Any(r => r.Id == childRole.Id), Is.False); // Exclude child role
        Assert.That(result.Any(r => r.Id == unrelatedRole.Id), Is.True); // Include unrelated role
    }
}