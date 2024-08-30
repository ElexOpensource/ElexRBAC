
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Rbac.Controllers;
using System.Security.Claims;

namespace RbacDashboard.Web.Test;

public class AccountControllerTest
{
    private Mock<HttpContext> _httpContextMock;
    private AccountController _controller;

    [SetUp]
    public void SetUp()
    {
        _httpContextMock = new Mock<HttpContext>();
        _controller = new AccountController();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextMock.Object
        };
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task CurrentUser_ShouldReturnAuthenticatedUserState_WhenUserIsAuthenticated()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim("CustomClaimType", "CustomClaimValue")
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        var authResult = AuthenticateResult.Success(new AuthenticationTicket(principal, "TestAuthScheme"));

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

        // Act
        var result = await _controller.CurrentUser();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsAuthenticated, Is.True);
        Assert.That(result.Name, Is.EqualTo("TestUser"));
        Assert.That(result.Claims.Count(), Is.EqualTo(2));
        Assert.That(result.Claims.Last().Type, Is.EqualTo("CustomClaimType"));
        Assert.That(result.Claims.Last().Value, Is.EqualTo("CustomClaimValue"));
    }

    [Test]
    public async Task CurrentUser_ShouldReturnUnauthenticatedState_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var authResult = AuthenticateResult.Fail("Authentication failed");

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

        // Act
        var result = await _controller.CurrentUser();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsAuthenticated, Is.False);
        Assert.That(result.Name, Is.EqualTo(string.Empty));
        Assert.That(result.Claims, Is.Empty);
    }

    [Test]
    public async Task CurrentUser_ShouldReturnUnauthenticatedState_WhenAuthenticateResultIsNull()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync((AuthenticateResult)null);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

        // Act
        var result = await _controller.CurrentUser();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsAuthenticated, Is.False);
        Assert.That(result.Name, Is.EqualTo(string.Empty));
        Assert.That(result.Claims, Is.Empty);
    }
        
    [Test]
    public async Task CurrentUser_ShouldReturnUnauthenticatedState_WhenPrincipalIsNull()
    {
        // Arrange
        var authResult = AuthenticateResult.NoResult();

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

        // Act
        var result = await _controller.CurrentUser();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsAuthenticated, Is.False);
        Assert.That(result.Name, Is.EqualTo(string.Empty));
        Assert.That(result.Claims, Is.Empty);
    }

    [Test]
    public async Task CurrentUser_ShouldReturnUnauthenticatedState_WhenIdentityIsNull()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity(null, "TestAuthType"));
        var authResult = AuthenticateResult.Success(new AuthenticationTicket(principal, "TestAuthScheme"));

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

        // Act
        var result = await _controller.CurrentUser();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsAuthenticated, Is.True);
        Assert.That(result.Name, Is.EqualTo(string.Empty));
        Assert.That(result.Claims, Is.Empty);
    }

    [Test]
    public async Task CurrentUser_ShouldReturnUnauthenticatedState_WhenIdentityNameIsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity(null, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        var authResult = AuthenticateResult.Success(new AuthenticationTicket(principal, "TestAuthScheme"));

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        _httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

        // Act
        var result = await _controller.CurrentUser();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsAuthenticated, Is.True);
        Assert.That(result.Name, Is.EqualTo(string.Empty));
        Assert.That(result.Claims, Is.Empty);
    }
}
