using Moq;
using RbacDashboard.Common.Interface;
using RbacDashboard.Common;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Commands;
using RbacDashboard.BAL;

namespace RbacDashboard.Test.BAL;

public class AccessRepositoryTest
{
    private Mock<IMediatorService> _mediatorMock;
    private Mock<IRbacTokenRepository> _tokenRepositoryMock;
    private AccessTokenRepository _accessTokenRepository;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediatorService>();
        _tokenRepositoryMock = new Mock<IRbacTokenRepository>();
        _accessTokenRepository = new AccessTokenRepository(_mediatorMock.Object, _tokenRepositoryMock.Object);
    }

    [Test]
    public async Task GetByRoleIds_ShouldReturnJwtToken_WhenRoleAccessesExist()
    {
        // Arrange
        var roleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var roleAccesses = new List<RoleAccess>
            {
                new RoleAccess
                {
                    RoleId = roleIds[0],
                    AccessId = Guid.NewGuid(),
                    Access = new Access { Name = "TestAccess1", MetaData = "{\"Permissions\": [{\"Id\": \"" + Guid.NewGuid() + "\", \"Name\": \"Read\"}], \"CanInherit\": true, \"GenerateToken\": true}" }
                },
                new RoleAccess
                {
                    RoleId = roleIds[1],
                    AccessId = Guid.NewGuid(),
                    Access = new Access { Name = "TestAccess2", MetaData = "{\"Permissions\": [{\"Id\": \"" + Guid.NewGuid() + "\", \"Name\": \"Read\"}], \"CanInherit\": true, \"GenerateToken\": true}" }
                }
            };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessByRoleIds>()))
            .ReturnsAsync(roleAccesses);

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleById>()))
            .ReturnsAsync(new Role() { ApplicationId = Guid.NewGuid() });

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(new List<Role>() { new Role { Id = roleIds.First(), Name = "Role one" }, new Role { Id = roleIds.Last(), Name = "Role two" } });

        _tokenRepositoryMock.Setup(t => t.GenerateJwtToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Returns("test-token");

        // Act
        var result = await _accessTokenRepository.GetByRoleIds(roleIds);

        // Assert
        _tokenRepositoryMock.Verify(t => t.GenerateJwtToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        Assert.That(result, Is.EqualTo("test-token"));
    }

    [Test]
    public async Task GetByRoleIds_ShouldReturnEmptyString_WhenNoRoleAccessesExist()
    {
        // Arrange
        var roleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessByRoleIds>()))
            .ReturnsAsync(new List<RoleAccess>());

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleById>()))
            .ReturnsAsync(new Role() { ApplicationId = Guid.NewGuid() });

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(new List<Role>());

        // Act
        var result = await _accessTokenRepository.GetByRoleIds(roleIds);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task GetByRoleIds_ShouldReturnJwtToken_WhenAccessMetaDataExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var roleIds = new List<Guid> { roleId };
        var applicationId = Guid.NewGuid();

        var role = new Role { Id = roleId, ApplicationId = applicationId, Name = "Role 1" };
        var allRoles = new List<Role>
        {
            role,
            new Role { Id = Guid.NewGuid(), ParentId = roleId, Name = "Child Role" },
        };

        var roleAccess = new RoleAccess
        {
            RoleId = roleId,
            AccessId = Guid.NewGuid(),
            Access = new Access { Name = "Access 1", MetaData = "{\"Permissions\": [{\"Id\": \"" + Guid.NewGuid() + "\", \"Name\": \"Read\"}], \"CanInherit\": true, \"GenerateToken\": true}" }
        };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleById>()))
            .ReturnsAsync(role);
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(allRoles);
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessByRoleIds>()))
            .ReturnsAsync(new List<RoleAccess> { roleAccess });

        _tokenRepositoryMock.Setup(t => t.GenerateJwtToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Returns("test-token");

        // Act
        var result = await _accessTokenRepository.GetByRoleIds(roleIds);

        // Assert
        Assert.That(result, Is.EqualTo("test-token"));
    }

    [Test]
    public async Task GetByRoleIds_ShouldReturnEmptyString_WhenNoAccessMetaDataExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var roleIds = new List<Guid> { roleId };
        var applicationId = Guid.NewGuid();

        var role = new Role { Id = roleId, ApplicationId = applicationId, Name = "Role 1" };
        var allRoles = new List<Role>
        {
            role,
            new Role { Id = Guid.NewGuid(), ParentId = roleId, Name = "Child Role" }
        };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleById>()))
            .ReturnsAsync(role);
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(allRoles);
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessByRoleIds>()))
            .ReturnsAsync(new List<RoleAccess>());

        // Act
        var result = await _accessTokenRepository.GetByRoleIds(roleIds);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }
}
