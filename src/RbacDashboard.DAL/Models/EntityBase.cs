using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models;


[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public class EntityBase
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }
}
