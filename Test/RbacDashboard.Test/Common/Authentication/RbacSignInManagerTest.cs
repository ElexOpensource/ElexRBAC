

using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RbacDashboard.Common.Authentication;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;
using System.Security.Claims;

namespace RbacDashboard.Common.Test;

public class RbacSignInManagerTest
{
    private Mock<UserManager<RbacApplicationUser>> _userManagerMock;
    private Mock<IHttpContextAccessor> _contextAccessorMock;
    private Mock<IUserClaimsPrincipalFactory<RbacApplicationUser>> _claimsFactoryMock;
    private Mock<IOptions<IdentityOptions>> _optionsAccessorMock;
    private Mock<ILogger<SignInManager<RbacApplicationUser>>> _loggerMock;
    private Mock<IAuthenticationSchemeProvider> _schemesMock;
    private Mock<IUserConfirmation<RbacApplicationUser>> _confirmationMock;
    private Mock<IMediatorService> _mediatorMock;
    private Mock<IRbacTokenRepository> _tokenRepositoryMock;
    private Mock<HttpContext> _httpContextMock;
    private Mock<IRequestCookieCollection> _cookiesMock;
    private RbacSignInManager _signInManager;

    [SetUp]
    public void Setup()
    {
        // Mock the dependencies for UserManager
        var storeMock = new Mock<IUserStore<RbacApplicationUser>>();
        var optionsMock = new Mock<IOptions<IdentityOptions>>();
        var passwordHasherMock = new Mock<IPasswordHasher<RbacApplicationUser>>();
        var userValidatorsMock = new List<IUserValidator<RbacApplicationUser>> { new Mock<IUserValidator<RbacApplicationUser>>().Object };
        var passwordValidatorsMock = new List<IPasswordValidator<RbacApplicationUser>> { new Mock<IPasswordValidator<RbacApplicationUser>>().Object };
        var keyNormalizerMock = new Mock<ILookupNormalizer>();
        var errorsMock = new Mock<IdentityErrorDescriber>();
        var servicesMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<UserManager<RbacApplicationUser>>>();

        _userManagerMock = new Mock<UserManager<RbacApplicationUser>>(
            storeMock.Object, optionsMock.Object, passwordHasherMock.Object,
            userValidatorsMock, passwordValidatorsMock, keyNormalizerMock.Object,
            errorsMock.Object, servicesMock.Object, loggerMock.Object);

        _contextAccessorMock = new Mock<IHttpContextAccessor>();
        _claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<RbacApplicationUser>>();
        _optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
        _loggerMock = new Mock<ILogger<SignInManager<RbacApplicationUser>>>();
        _schemesMock = new Mock<IAuthenticationSchemeProvider>();
        _confirmationMock = new Mock<IUserConfirmation<RbacApplicationUser>>();
        _mediatorMock = new Mock<IMediatorService>();
        _tokenRepositoryMock = new Mock<IRbacTokenRepository>();

        _httpContextMock = new Mock<HttpContext>();
        _cookiesMock = new Mock<IRequestCookieCollection>();
        _contextAccessorMock.Setup(a => a.HttpContext).Returns(_httpContextMock.Object);
        _httpContextMock.Setup(c => c.Request.Cookies).Returns(_cookiesMock.Object);

        _signInManager = new RbacSignInManager(_userManagerMock.Object, _contextAccessorMock.Object,
            _claimsFactoryMock.Object, _optionsAccessorMock.Object, _loggerMock.Object,
            _schemesMock.Object, _confirmationMock.Object, _mediatorMock.Object, _tokenRepositoryMock.Object);
    }

    [Test]
    public async Task PasswordSignInAsync_ShouldReturnSuccess_WhenValidUser()
    {
        // Arrange
        var userId = "validUserId";
        var appId = "validAppId";
        var isPersistent = false;
        var lockoutOnFailure = false;

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);
        principalMock.Setup(p => p.FindFirst(It.Is<string>(s => s == RbacConstants.CustomerId)))
             .Returns(new Claim(RbacConstants.CustomerId, Guid.NewGuid().ToString()));

        var claimsIdentityMock = new Mock<ClaimsIdentity>();
        claimsIdentityMock.Setup(ci => ci.IsAuthenticated).Returns(true);
        claimsIdentityMock.Setup(ci => ci.Claims).Returns(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(RbacConstants.ApplicationId, "application-id-value"),
        });

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        var customer = new Customer { Id = Guid.NewGuid(), CustomerName = "Test Customer" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
        .ReturnsAsync(customer);

        _claimsFactoryMock.Setup(m => m.CreateAsync(It.IsAny<RbacApplicationUser>())).ReturnsAsync(new ClaimsPrincipal(claimsIdentityMock.Object));

        var authenticationServiceMock = new Mock<IAuthenticationService>();
        authenticationServiceMock
            .Setup(auth => auth.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);

        _contextAccessorMock.Setup(ca => ca.HttpContext).Returns(httpContextMock.Object);


        // Act
        var result = await _signInManager.PasswordSignInAsync(userId, appId, isPersistent, lockoutOnFailure);

        // Assert
        Assert.That(result, Is.EqualTo(SignInResult.Success));
    }

    [Test]
    public async Task PasswordSignInAsync_ShouldReturnFailier_WhencustomerIdNotinClaim()
    {
        // Arrange
        var userId = "validUserId";
        var appId = "validAppId";
        var isPersistent = false;
        var lockoutOnFailure = false;

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        var customer = new Customer { Id = Guid.NewGuid(), CustomerName = "Test Customer" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
                     .ReturnsAsync(customer);

        // Act
        var result = await _signInManager.PasswordSignInAsync(userId, appId, isPersistent, lockoutOnFailure);

        // Assert
        Assert.That(result, Is.EqualTo(SignInResult.Failed));
    }

    [Test]
    public async Task PasswordSignInAsync_ShouldReturnFailier_WhencustomerNotFound()
    {
        // Arrange
        var userId = "validUserId";
        var appId = "validAppId";
        var isPersistent = false;
        var lockoutOnFailure = false;

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);
        principalMock.Setup(p => p.FindFirst(It.Is<string>(s => s == RbacConstants.CustomerId)))
            .Returns(new Claim(RbacConstants.CustomerId, Guid.NewGuid().ToString()));

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
                     .ReturnsAsync((Customer)null);

        // Act
        var result = await _signInManager.PasswordSignInAsync(userId, appId, isPersistent, lockoutOnFailure);

        // Assert
        Assert.That(result, Is.EqualTo(SignInResult.Failed));
    }

    [Test]
    public async Task PasswordSignInAsync_ShouldReturnFailier_WhenExceptionOccurs()
    {
        // Arrange
        var userId = "validUserId";
        var appId = "validAppId";
        var isPersistent = false;
        var lockoutOnFailure = false;

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);
        principalMock.Setup(p => p.FindFirst(It.Is<string>(s => s == RbacConstants.CustomerId)))
            .Returns(new Claim(RbacConstants.CustomerId, Guid.NewGuid().ToString()));

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
                     .ThrowsAsync(new Exception());

        // Act
        var result = await _signInManager.PasswordSignInAsync(userId, appId, isPersistent, lockoutOnFailure);

        // Assert
        Assert.That(result, Is.EqualTo(SignInResult.Failed));
    }

    [Test]
    public async Task PasswordSignInAsync_ShouldReturnFailed_WhenInvalidToken()
    {
        // Arrange
        var userId = "invalidUserId";
        var appId = "invalidAppId";
        var isPersistent = false;
        var lockoutOnFailure = false;

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(false);
        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        // Act
        var result = await _signInManager.PasswordSignInAsync(userId, appId, isPersistent, lockoutOnFailure);

        // Assert
        Assert.That(result, Is.EqualTo(SignInResult.Failed));
    }

    [Test]
    public async Task SignInWithClaimsAsync_ShouldSignInUserWithClaims()
    {
        // Arrange
        var userId = "validUserId";
        var user = new RbacApplicationUser { CustomerId = Guid.NewGuid().ToString(), CustomerName = "cusName", ApplicationId = Guid.NewGuid().ToString(), ApplicationName = "appName" };
        var additionalClaims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };

        var authProperties = new AuthenticationProperties();

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);
        principalMock.Setup(p => p.FindFirst(It.Is<string>(s => s == RbacConstants.CustomerId)))
             .Returns(new Claim(RbacConstants.CustomerId, Guid.NewGuid().ToString()));

        var claimsIdentityMock = new Mock<ClaimsIdentity>();
        claimsIdentityMock.Setup(ci => ci.IsAuthenticated).Returns(true);
        claimsIdentityMock.Setup(ci => ci.Claims).Returns(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(RbacConstants.ApplicationId, "application-id-value"),
        });

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        var customer = new Customer { Id = Guid.NewGuid(), CustomerName = "Test Customer" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
        .ReturnsAsync(customer);

        _claimsFactoryMock.Setup(m => m.CreateAsync(It.IsAny<RbacApplicationUser>())).ReturnsAsync(new ClaimsPrincipal(claimsIdentityMock.Object));

        var authenticationServiceMock = new Mock<IAuthenticationService>();
        authenticationServiceMock
            .Setup(auth => auth.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);
        _contextAccessorMock.Setup(ca => ca.HttpContext).Returns(httpContextMock.Object);

        // Act
        var result = await _signInManager.SignInWithClaimsAsync(user, authProperties, additionalClaims);

        // Assert
        authenticationServiceMock.Verify(auth => auth.SignInAsync(httpContextMock.Object, RbacConstants.AuthenticationSchema, It.IsAny<ClaimsPrincipal>(), authProperties), Times.Once);
        Assert.That(result, Is.EqualTo(SignInResult.Success));
    }

    [Test]
    public async Task RefreshSignInAsync_ShouldUpdateSignInState_WhenUserIsValid()
    {
        // Arrange
        var userId = "validUserId";
        var user = new RbacApplicationUser { CustomerId = Guid.NewGuid().ToString(), CustomerName = "cusName", ApplicationId = Guid.NewGuid().ToString(), ApplicationName = "appName" };
        var additionalClaims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };

        var authProperties = new AuthenticationProperties();

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);
        principalMock.Setup(p => p.FindFirst(It.Is<string>(s => s == RbacConstants.CustomerId)))
             .Returns(new Claim(RbacConstants.CustomerId, Guid.NewGuid().ToString()));

        var claimsIdentityMock = new Mock<ClaimsIdentity>();
        claimsIdentityMock.Setup(ci => ci.IsAuthenticated).Returns(true);
        claimsIdentityMock.Setup(ci => ci.Claims).Returns(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(RbacConstants.ApplicationId, "application-id-value"),
        });

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        var customer = new Customer { Id = Guid.NewGuid(), CustomerName = "Test Customer" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
        .ReturnsAsync(customer);

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetApplicationById>()))
        .ReturnsAsync(new Application() { Id = Guid.NewGuid(), ApplicationName = "App Name"});

        _claimsFactoryMock.Setup(m => m.CreateAsync(It.IsAny<RbacApplicationUser>())).ReturnsAsync(new ClaimsPrincipal(claimsIdentityMock.Object));

        var authenticationServiceMock = new Mock<IAuthenticationService>();
        authenticationServiceMock
            .Setup(auth => auth.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);
        _contextAccessorMock.Setup(ca => ca.HttpContext).Returns(httpContextMock.Object);

        // Act
        await _signInManager.RefreshSignInAsync(user);

        // Assert
        authenticationServiceMock.Verify(auth => auth.SignInAsync(It.IsAny<HttpContext>(), RbacConstants.AuthenticationSchema, It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task RefreshSignInAsync_ShouldNotUpdateSignInState_WhenApplicationIdIsNull()
    {
        // Arrange
        var userId = "validUserId";
        var user = new RbacApplicationUser { CustomerId = Guid.NewGuid().ToString(), CustomerName = "cusName", ApplicationId = null, ApplicationName = "appName" };
        var additionalClaims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };

        var authProperties = new AuthenticationProperties();

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);
        principalMock.Setup(p => p.FindFirst(It.Is<string>(s => s == RbacConstants.CustomerId)))
             .Returns(new Claim(RbacConstants.CustomerId, Guid.NewGuid().ToString()));

        var claimsIdentityMock = new Mock<ClaimsIdentity>();
        claimsIdentityMock.Setup(ci => ci.IsAuthenticated).Returns(true);
        claimsIdentityMock.Setup(ci => ci.Claims).Returns(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(RbacConstants.ApplicationId, "application-id-value"),
        });

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        var customer = new Customer { Id = Guid.NewGuid(), CustomerName = "Test Customer" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
        .ReturnsAsync(customer);

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetApplicationById>()))
        .ReturnsAsync(new Application() { Id = Guid.NewGuid(), ApplicationName = "App Name" });

        _claimsFactoryMock.Setup(m => m.CreateAsync(It.IsAny<RbacApplicationUser>())).ReturnsAsync(new ClaimsPrincipal(claimsIdentityMock.Object));

        var authenticationServiceMock = new Mock<IAuthenticationService>();
        authenticationServiceMock
            .Setup(auth => auth.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);
        _contextAccessorMock.Setup(ca => ca.HttpContext).Returns(httpContextMock.Object);

        // Act
        await _signInManager.RefreshSignInAsync(user);

        // Assert
        authenticationServiceMock.Verify(auth => auth.SignInAsync(It.IsAny<HttpContext>(), RbacConstants.AuthenticationSchema, It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Never);
    }

    [Test]
    public async Task RefreshSignInAsync_ShouldNotUpdateSignInState_WhenCustomerNotFound()
    {
        // Arrange
        var userId = "validUserId";
        var user = new RbacApplicationUser { CustomerId = Guid.NewGuid().ToString(), CustomerName = "cusName", ApplicationId = null, ApplicationName = "appName" };
        var additionalClaims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };

        var authProperties = new AuthenticationProperties();

        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(p => p.Identity.IsAuthenticated).Returns(true);
        principalMock.Setup(p => p.FindFirst(It.Is<string>(s => s == RbacConstants.CustomerId)))
             .Returns(new Claim(RbacConstants.CustomerId, Guid.NewGuid().ToString()));

        var claimsIdentityMock = new Mock<ClaimsIdentity>();
        claimsIdentityMock.Setup(ci => ci.IsAuthenticated).Returns(true);
        claimsIdentityMock.Setup(ci => ci.Claims).Returns(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(RbacConstants.ApplicationId, "application-id-value"),
        });

        _tokenRepositoryMock.Setup(t => t.ValidateJwtToken(userId)).Returns(principalMock.Object);

        var customer = new Customer { Id = Guid.NewGuid(), CustomerName = "Test Customer" };
        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetCustomerById>()))
        .ReturnsAsync((Customer) null);

        _mediatorMock.Setup(m => m.SendRequest(It.IsAny<GetApplicationById>()))
        .ReturnsAsync(new Application() { Id = Guid.NewGuid(), ApplicationName = "App Name" });

        _claimsFactoryMock.Setup(m => m.CreateAsync(It.IsAny<RbacApplicationUser>())).ReturnsAsync(new ClaimsPrincipal(claimsIdentityMock.Object));

        var authenticationServiceMock = new Mock<IAuthenticationService>();
        authenticationServiceMock
            .Setup(auth => auth.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);
        _contextAccessorMock.Setup(ca => ca.HttpContext).Returns(httpContextMock.Object);

        // Act
        await _signInManager.RefreshSignInAsync(user);

        // Assert
        authenticationServiceMock.Verify(auth => auth.SignInAsync(It.IsAny<HttpContext>(), RbacConstants.AuthenticationSchema, It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Never);
    }

    [Test]
    public async Task SignOutAsync_ShouldSignOutUser()
    {
        // Arrange
        var authenticationServiceMock = new Mock<IAuthenticationService>();
        authenticationServiceMock
            .Setup(auth => auth.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationServiceMock.Object);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(h => h.RequestServices).Returns(serviceProviderMock.Object);
        _contextAccessorMock.Setup(ca => ca.HttpContext).Returns(httpContextMock.Object);

        // Act
        await _signInManager.SignOutAsync();

        // Assert
        authenticationServiceMock.Verify(auth => auth.SignOutAsync(
            httpContextMock.Object,
            RbacConstants.AuthenticationSchema,
            null), Times.Once);
    }
}
