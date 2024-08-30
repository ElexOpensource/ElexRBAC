using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models.Domain;

[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public class AccessJSON
{
    public required List<Option> Permissions { get; set; }

    public bool CanInherit { get; set; }

    public bool GenerateToken { get; set; }
}