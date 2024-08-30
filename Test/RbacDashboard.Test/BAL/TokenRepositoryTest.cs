
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace RbacDashboard.BAL.Test;

public class TokenRepositoryTest
{
    private Mock<IConfiguration> _configurationMock;
    private TokenRepository _tokenRepository;

    [SetUp]
    public void SetUp()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["RbacSettings:Jwt:IssuerSigningKey"]).Returns("aGVsbG8gd29ybGQgdGhpcyBpcyBhIHRlc3Qgc3RyaW5n");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidIssuer"]).Returns("ValidIssuer");
        _configurationMock.Setup(c => c["RbacSettings:Jwt:ValidAudience"]).Returns("ValidAudience");

        _tokenRepository = new TokenRepository(_configurationMock.Object);
    }

    [Test]
    public void GenerateJwtToken_ShouldReturnValidToken_WhenCalled()
    {
        // Arrange
        var accessMetaDataJson = "{\"Permissions\":[{\"Id\":\"123\"}]}";
        var claimName = "AccessMetaData";
        var expiresDays = 1;

        // Act
        var token = _tokenRepository.GenerateJwtToken(accessMetaDataJson, claimName, expiresDays);

        // Assert
        Assert.That(token, Is.Not.Null.And.Not.Empty);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.That(jwtToken.Claims.First(c => c.Type == claimName).Value, Is.EqualTo(accessMetaDataJson));
    }

    [Test]
    public void ReadToken_ShouldReturnClaimValue_WhenCalledWithValidToken()
    {
        // Arrange
        var accessMetaDataJson = "{\"Permissions\":[{\"Id\":\"123\"}]}";
        var claimName = "AccessMetaData";
        var token = _tokenRepository.GenerateJwtToken(accessMetaDataJson, claimName);

        // Act
        var result = _tokenRepository.ReadToken(token, claimName);

        // Assert
        Assert.That(result, Is.EqualTo(accessMetaDataJson));
    }

    [Test]
    public void ReadToken_ShouldReturnEmptyString_WhenCalledWithInValidTokenOrClimeName()
    {
        // Arrange
        var accessMetaDataJson = "{\"Permissions\":[{\"Id\":\"123\"}]}";
        var claimName = "AccessMetaData";
        var token = _tokenRepository.GenerateJwtToken(accessMetaDataJson, claimName);

        // Act
        var result = _tokenRepository.ReadToken(token, "AccessMetaDataToken");

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ValidateJwtToken_ShouldReturnValidClaimsPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var accessMetaDataJson = "{\"Permissions\":[{\"Id\":\"123\"}]}";
        var token = _tokenRepository.GenerateJwtToken(accessMetaDataJson);

        // Act
        var claimsPrincipal = _tokenRepository.ValidateJwtToken(token);

        // Assert
        Assert.That(claimsPrincipal, Is.Not.Null);
        Assert.That(claimsPrincipal.Claims.Any(c => c.Type == "AccessMetaData" && c.Value == accessMetaDataJson), Is.True);
    }

    [Test]
    public void ValidateJwtToken_ShouldThrowSecurityTokenValidationException_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "ThisIsAnInvalidToken";

        // Act & Assert
        var ex = Assert.Throws<SecurityTokenMalformedException>(() => _tokenRepository.ValidateJwtToken(invalidToken));
        Assert.That(ex.Message, Is.Not.Null);
    }
}
