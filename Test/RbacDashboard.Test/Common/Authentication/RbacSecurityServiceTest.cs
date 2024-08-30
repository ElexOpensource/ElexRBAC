
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using RbacDashboard.Common.Authentication;
using System.Security.Claims;

namespace RbacDashboard.Common.Test;

public class RbacSecurityServiceTest
{
    private Mock<NavigationManager> _navigationManagerMock;
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private HttpClient _httpClient;
    private RbacSecurityService _securityService;

    [SetUp]
    public void Setup()
    {

        _navigationManagerMock = new Mock<NavigationManager>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>(); 
        _httpClient = new HttpClient(new Mock<HttpMessageHandler>().Object);

        _httpClientFactoryMock
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(_httpClient);

        _securityService = new RbacSecurityService(
            _navigationManagerMock.Object,
            _httpClientFactoryMock.Object);
    }

    [TearDown]
    public void TearDownDispose()
    {
        _httpClient.Dispose();
    }

    [Test]
    public void InitializeAsync_ShouldSetUserProperties_WhenAuthenticated()
    {
        // Arrange
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "customer-id"),
        new Claim(ClaimTypes.Name, "customer-name"),
        new Claim(RbacConstants.ApplicationId, "app-id"),
        new Claim(RbacConstants.ApplicationName, "app-name")
    };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Rbac"));
        var authenticationState = new AuthenticationState(claimsPrincipal);

        // Act
        var result = _securityService.InitializeAsync(authenticationState);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_securityService.User.CustomerId, Is.EqualTo("customer-id"));
        Assert.That(_securityService.User.CustomerName, Is.EqualTo("customer-name"));
        Assert.That(_securityService.User.ApplicationId, Is.EqualTo("app-id"));
        Assert.That(_securityService.User.ApplicationName, Is.EqualTo("app-name"));
    }

    [Test]
    public void GetCustomerId_ShouldReturnCustomerId_WhenAuthenticated()
    {
        // Arrange
        _securityService.InitializeAsync(new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, "customer-id")
            }, "Rbac"))));

        // Act
        var customerId = _securityService.GetCustomerId();

        // Assert
        Assert.That(customerId, Is.EqualTo("customer-id"));
    }

    [Test]
    public void GetApplicationId_ShouldReturnApplicationId_WhenAuthenticated()
    {
        // Arrange
        _securityService.InitializeAsync(new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
            new Claim(RbacConstants.ApplicationId, "app-id")
            }, "Rbac"))));

        // Act
        var appId = _securityService.GetApplicationId();

        // Assert

        Assert.That(appId, Is.EqualTo("app-id"));
    }
}
