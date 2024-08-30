
using Moq;
using RbacDashboard.BAL;
using RbacDashboard.Common;
using RbacDashboard.DAL.Models.Domain;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Commands;

namespace RbacDashboard.BAL.Test;

public class RoleAccessRepositorTest
{
    private Mock<IMediatorService> _mediatorMock;
    private RoleAccessRepository _roleAccessRepository;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediatorService>();
        _roleAccessRepository = new RoleAccessRepository(_mediatorMock.Object);
    }

    [Test]
    public async Task GetByRoleId_ShouldReturnRoleAccesses()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var roleAccesses = new List<RoleAccess>
            {
                new RoleAccess { RoleId = roleId, AccessId = Guid.NewGuid(), IsActive = true },
                new RoleAccess { RoleId = roleId, AccessId = Guid.NewGuid(), IsActive = true }
            };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessesByRoleId>())).ReturnsAsync(roleAccesses);

        // Act
        var result = await _roleAccessRepository.GetByRoleId(roleId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetRoleAccessesByRoleId>(req => req.RoleId == roleId)), Times.Once);
        Assert.That(result, Is.EqualTo(roleAccesses));
    }

    [Test]
    public async Task AddRemoveAccess_ShouldProcessAddAndRemoveAccesses()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var addAccessId = Guid.NewGuid();
        var removeAccessId = Guid.NewGuid();

        var accessRequest = new AddRemoveAccessRequest
        {
            RoleId = roleId,
            AddAccess = new List<Guid> { addAccessId },
            RemoveAccess = new List<Guid> { removeAccessId }
        };

        var roleAccess = new RoleAccess();

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessesByRoleAndAccessId>())).ReturnsAsync(roleAccess);
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<AddorUpdateRoleAccess>())).ReturnsAsync(new RoleAccess());

        // Act
        await _roleAccessRepository.AddRemoveAccess(applicationId, accessRequest);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetRoleAccessesByRoleAndAccessId>(req => req.RoleId == roleId && req.AccessId == addAccessId)), Times.Once);
        _mediatorMock.Verify(m => m.SendRequest(It.Is<AddorUpdateRoleAccess>(req => req.RoleAccess != null)), Times.AtLeastOnce);
    }

    [Test]
    public async Task AddorUpdate_ShouldReturnUpdatedRoleAccess()
    {
        // Arrange
        var roleAccess = new RoleAccess { RoleId = Guid.NewGuid(), AccessId = Guid.NewGuid() };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<AddorUpdateRoleAccess>())).ReturnsAsync(roleAccess);

        // Act
        var result = await _roleAccessRepository.AddorUpdate(roleAccess);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<AddorUpdateRoleAccess>(req => req.RoleAccess == roleAccess)), Times.Once);
        Assert.That(result, Is.EqualTo(roleAccess));
    }

    [Test]
    public async Task Delete_ShouldCallDeleteRoleAccess()
    {
        // Arrange
        var roleAccessId = Guid.NewGuid();

        // Act
        await _roleAccessRepository.Delete(roleAccessId);

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<DeleteRoleAccess>(req => req.Id == roleAccessId)), Times.Once);
    }

    [Test]
    public async Task ProcessAddAccess_ShouldAddNewRoleAccess_WhenRoleAccessDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var accessId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessesByRoleAndAccessId>())).ReturnsAsync((RoleAccess)null);
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<AddorUpdateRoleAccess>())).ReturnsAsync(new RoleAccess());

        // Act
        await _roleAccessRepository.AddRemoveAccess(applicationId, new AddRemoveAccessRequest
        {
            RoleId = roleId,
            AddAccess = new List<Guid> { accessId },
            RemoveAccess = new List<Guid>()
        });

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetRoleAccessesByRoleAndAccessId>(req => req.RoleId == roleId && req.AccessId == accessId)), Times.Once);
        _mediatorMock.Verify(m => m.SendRequest(It.Is<AddorUpdateRoleAccess>(req => req.RoleAccess.RoleId == roleId && req.RoleAccess.AccessId == accessId && req.RoleAccess.ApplicationId == applicationId)), Times.Once);
    }

    [Test]
    public async Task ProcessRemoveAccess_ShouldDeactivateRoleAccess_WhenRoleAccessExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var accessId = Guid.NewGuid();
        var roleAccess = new RoleAccess { RoleId = roleId, AccessId = accessId, IsActive = true };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessesByRoleAndAccessId>())).ReturnsAsync(roleAccess);
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<AddorUpdateRoleAccess>())).ReturnsAsync(roleAccess);

        // Act
        await _roleAccessRepository.AddRemoveAccess(Guid.NewGuid(), new AddRemoveAccessRequest
        {
            RoleId = roleId,
            AddAccess = new List<Guid>(),
            RemoveAccess = new List<Guid> { accessId }
        });

        // Assert
        _mediatorMock.Verify(m => m.SendRequest(It.Is<GetRoleAccessesByRoleAndAccessId>(req => req.RoleId == roleId && req.AccessId == accessId)), Times.Once);
        _mediatorMock.Verify(m => m.SendRequest(It.Is<AddorUpdateRoleAccess>(req => req.RoleAccess.RoleId == roleId && req.RoleAccess.AccessId == accessId && req.RoleAccess.IsActive == false)), Times.Once);
    }
}
