using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Common;

[ExcludeFromCodeCoverage(Justification = "This class is used like a constant, so there's no need to test it.")]
public static class RbacConstants
{
    public const string AuthenticationSchema = "RbacAuthenticationSchema";
    public const string CustomerId = "CID";
    public const string CustomerName = "CustomerName";
    public const string ApplicationId = "AID";
    public const string ApplicationName = "ApplicationName";
    public const int CookieLifeSpanInMinutes = 120;
}
