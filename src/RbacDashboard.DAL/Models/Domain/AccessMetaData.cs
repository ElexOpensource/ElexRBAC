using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models.Domain;

[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public class AccessMetaData : AccessJSON
{
    public Guid AccessId { get; set; }

    public required string AccessName { get; set; }
}