using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models.Domain;

public class Option
{
    public required Guid Id { get; set; }

    public required string Label { get; set; }
}
