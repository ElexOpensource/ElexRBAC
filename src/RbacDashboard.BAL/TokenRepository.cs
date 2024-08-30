using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RbacDashboard.Common.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RbacDashboard.BAL;

public class TokenRepository(IConfiguration configuration) : IRbacTokenRepository
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateJwtToken(string accessMetaDataJson, string claimName = "AccessMetaData", int expiresDays = 1)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["RbacSettings:Jwt:IssuerSigningKey"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(claimName, accessMetaDataJson)
            ]),
            Issuer = _configuration["RbacSettings:Jwt:ValidIssuer"],
            Audience = _configuration["RbacSettings:Jwt:ValidAudience"],
            Expires = DateTime.UtcNow.AddDays(expiresDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        return token;
    }

    public string ReadToken(string token, string claimName = "AccessMetaData")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var modelJson = jwtToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value ?? string.Empty;
        return modelJson;
    }

    public ClaimsPrincipal ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["RbacSettings:Jwt:IssuerSigningKey"]!);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["RbacSettings:Jwt:ValidIssuer"],
            ValidAudience = _configuration["RbacSettings:Jwt:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

        return principal;
    }
}
