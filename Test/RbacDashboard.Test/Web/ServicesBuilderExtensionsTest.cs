using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace RbacDashboard.Test.Web;

internal class ServicesBuilderExtensionsTest
{
    private Mock<IServiceCollection> _servicesMock;
    private Mock<IConfiguration> _configurationMock;
    private Mock<IWebHostEnvironment> _environmentMock;
    private Mock<IApplicationBuilder> _builderMock;
    private Mock<AuthorizationOptions> _authorizationOptionsMock;

    [SetUp]
    public void Setup()
    {
        _servicesMock = new Mock<IServiceCollection>();
        _configurationMock = new Mock<IConfiguration>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _builderMock = new Mock<IApplicationBuilder>();
        _authorizationOptionsMock = new Mock<AuthorizationOptions>();
    }

    [Test]
    public void AddRbacService_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();
        _configurationMock.Setup(c => c["RbacSettings:BaseUrl"]).Returns("http://example.com");
        _configurationMock.Setup(c => c["RbacSettings:DbConnectionString"]).Returns("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;");
        _configurationMock.Setup(c => c["RbacSettings:DbType"]).Returns("Sql");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidIssuer"]).Returns("issuer");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidAudience"]).Returns("audience");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:IssuerSigningKey"]).Returns("signingKey");

        // Act
        services.AddAuthorization(options => { options.AddRbacPolicy(); });
        services.AddRbacService(_configurationMock.Object, _environmentMock.Object);

        // Assert
        Assert.That(services.Any(s => s.ServiceType == typeof(IConfiguration)), Is.True);
        Assert.That(services.Any(s => s.ServiceType == typeof(IWebHostEnvironment)), Is.True);
    }

    [Test]
    public void AddRbacService_ShouldThrowArgumentNullException_WhenAnyConfigIsEmpty()
    {
        // Arrange
        var services = new ServiceCollection();
        _configurationMock.Setup(c => c["RbacSettings:BaseUrl"]).Returns("http://example.com");
        _configurationMock.Setup(c => c["RbacSettings:DbConnectionString"]).Returns("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;");
        _configurationMock.Setup(c => c["RbacSettings:DbType"]).Returns("SqlServer");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidIssuer"]).Returns("issuer");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidAudience"]).Returns("audience");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddRbacService(_configurationMock.Object, _environmentMock.Object));
    }

    [Test]
    public void AddRbacService_ShouldThrowArgumentNullException_WhenPolicyIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        _configurationMock.Setup(c => c["RbacSettings:BaseUrl"]).Returns("http://example.com");
        _configurationMock.Setup(c => c["RbacSettings:DbConnectionString"]).Returns("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;");
        _configurationMock.Setup(c => c["RbacSettings:DbType"]).Returns("SqlServer");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidIssuer"]).Returns("issuer");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidAudience"]).Returns("audience");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:IssuerSigningKey"]).Returns("signingKey");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => services.AddRbacService(_configurationMock.Object, _environmentMock.Object));
    }

    [Test]
    public void AddRbacService_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ServicesBuilderExtensions.AddRbacService(null!, _configurationMock.Object, _environmentMock.Object));
    }

    [Test]
    public void AddRbacService_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _servicesMock.Object.AddRbacService(null!, _environmentMock.Object));
    }

    [Test]
    public void AddRbacService_ShouldThrowArgumentNullException_WhenEnvironmentIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _servicesMock.Object.AddRbacService(_configurationMock.Object, null!));
    }

    [Test]
    public void AddRbacService_ShouldSetEnableSwaggerToFalse()
    {
        //Arrange
        var services = new ServiceCollection();
        _configurationMock.Setup(c => c["RbacSettings:BaseUrl"]).Returns("http://example.com");
        _configurationMock.Setup(c => c["RbacSettings:DbConnectionString"]).Returns("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;");
        _configurationMock.Setup(c => c["RbacSettings:DbType"]).Returns("Sql");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidIssuer"]).Returns("issuer");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidAudience"]).Returns("audience");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:IssuerSigningKey"]).Returns("signingKey");

        // Act
        services.AddAuthorization(options => { options.AddRbacPolicy(); });
        services.AddRbacService(_configurationMock.Object, _environmentMock.Object);

        // Assert
        Assert.That(ServicesBuilderExtensions._enableSwagger, Is.False);
    }
}
