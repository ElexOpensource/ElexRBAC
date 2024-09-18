
using Moq;
using Rbac.Controllers;
using RbacDashboard.Common.Interface;

namespace RbacDashboard.Test.Web;

public class AccessTokenControllerTest
{
    private Mock<IRbacAccessTokenRepository> _accessTokenRepositoryMock;
    private AccessTokenController _controller;

    [SetUp]
    public void SetUp()
    {
        _accessTokenRepositoryMock = new Mock<IRbacAccessTokenRepository>();
        _controller = new AccessTokenController(_accessTokenRepositoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task GetByRoleIds_ShouldReturnToken_WhenValidRoleIdsAreProvided()
    {
        // Arrange
        var roleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var expectedToken = "generatedAccessToken";

        _accessTokenRepositoryMock.Setup(repo => repo.GetByRoleIds(roleIds))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _controller.GetByRoleIds(roleIds);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(expectedToken, Is.EqualTo(result));
    }

    [Test]
    public async Task GetByRoleIds_ShouldReturnEmptyString_WhenNoRoleIdsAreProvided()
    {
        // Arrange
        var roleIds = new List<Guid>();
        var expectedToken = string.Empty;

        _accessTokenRepositoryMock.Setup(repo => repo.GetByRoleIds(roleIds))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _controller.GetByRoleIds(roleIds);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(expectedToken, Is.EqualTo(result));
    }

    [Test]
    public async Task GetByRoleIds_ShouldReturnEmptyString_WhenNullRoleIdsAreProvided()
    {
        // Arrange
        List<Guid> roleIds = null;
        var expectedToken = string.Empty;

        _accessTokenRepositoryMock.Setup(repo => repo.GetByRoleIds(roleIds))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _controller.GetByRoleIds(roleIds);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(expectedToken, Is.EqualTo(result));
    }
}
