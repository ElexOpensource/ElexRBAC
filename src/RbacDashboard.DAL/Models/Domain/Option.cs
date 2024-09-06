using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models.Domain;


[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public class Option
{
    public required Guid Id { get; set; }

    public required string Label { get; set; }
}


[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public class StatusOption
{
    public required bool Key { get; set; }

    public required string Value { get; set; }
}