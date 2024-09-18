using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models;

[Table("Permissionset", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class Permissionset : EntityBase
{
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public Guid PermissionTypeId { get; set; }

    public Guid? ParentId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Permissionset> InverseParent { get; set; } = new List<Permissionset>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Permissionset? Parent { get; set; }

    [ForeignKey("PermissionTypeId")]
    [InverseProperty("Permissionsets")]
    public virtual OptionsetMaster PermissionType { get; set; } = null!;
}