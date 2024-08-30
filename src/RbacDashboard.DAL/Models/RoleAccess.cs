using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models;

[Table("RoleAccess", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class RoleAccess
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid RoleId { get; set; }

    public Guid AccessId { get; set; }

    public string? AccessMetaData { get; set; }

    public Guid ApplicationId { get; set; }

    public bool IsOverwrite { get; set; }

    [ForeignKey("AccessId")]
    [InverseProperty("RoleAccesses")]
    public virtual Access? Access { get; set; } = null!;

    [ForeignKey("ApplicationId")]
    [InverseProperty("RoleAccesses")]
    public virtual Application? Application { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("RoleAccesses")]
    public virtual Role? Role { get; set; } = null!;
}