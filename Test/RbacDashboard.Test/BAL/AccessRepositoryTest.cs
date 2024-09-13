using Moq;
using RbacDashboard.Common.Interface;
using RbacDashboard.Common;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Commands;

namespace RbacDashboard.BAL.Test;

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
                    Access = new Access { AccessName = "TestAccess1", MetaData = "{\"Permissions\": [{\"Id\": \"" + Guid.NewGuid() + "\", \"Name\": \"Read\"}], \"CanInherit\": true, \"GenerateToken\": true}" }
                },
                new RoleAccess
                {
                    RoleId = roleIds[1],
                    AccessId = Guid.NewGuid(),
                    Access = new Access { AccessName = "TestAccess2", MetaData = "{\"Permissions\": [{\"Id\": \"" + Guid.NewGuid() + "\", \"Name\": \"Read\"}], \"CanInherit\": true, \"GenerateToken\": true}" }
                }
            };

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleAccessByRoleIds>()))
            .ReturnsAsync(roleAccesses);

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRoleById>()))
            .ReturnsAsync(new Role() { ApplicationId = Guid.NewGuid() });

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(new List<Role>() { new Role { Id = roleIds.First(), RoleName = "Role one" }, new Role { Id = roleIds.Last(), RoleName = "Role two" } });

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
            .ReturnsAsync(new Role() { ApplicationId = Guid.NewGuid()});

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetRolesByApplicationId>()))
            .ReturnsAsync(new List<Role>());

        // Act
        var result = await _accessTokenRepository.GetByRoleIds(roleIds);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }
}
