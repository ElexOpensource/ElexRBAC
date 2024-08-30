using RbacDashboard.Asserts;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Components.Layout;

[ExcludeFromCodeCoverage]
public partial class LoginLayout
{
    private readonly string backgroundImageUrl = $"_content/{typeof(RbacAsserts).Assembly.GetName().Name}/images/login.jpg";

    private static string Version => typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0";

    private static string Year
    {
        get
        {
            var buildDate = File.GetLastWriteTime(typeof(Program).Assembly.Location);
            return buildDate.Year.ToString();
        }
    }
}