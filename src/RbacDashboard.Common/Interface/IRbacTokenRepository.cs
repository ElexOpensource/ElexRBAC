using System.Security.Claims;

namespace RbacDashboard.Common.Interface;

public interface IRbacTokenRepository
{
    string GenerateJwtToken(string accessMetaDataJson, string claimName = "AccessMetaData", int expiresDays = 1);

    string ReadToken(string token, string claimName = "AccessMetaData");

    ClaimsPrincipal ValidateJwtToken(string token);
}
