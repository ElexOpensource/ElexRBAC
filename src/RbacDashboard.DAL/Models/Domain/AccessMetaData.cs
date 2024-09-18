using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models.Domain;

[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public class AccessMetaData 
{
    public Guid AccessId { get; set; }

    public required string AccessName { get; set; }

    public required List<Option> Permissions { get; set; }
}