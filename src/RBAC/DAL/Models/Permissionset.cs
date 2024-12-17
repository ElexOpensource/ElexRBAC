using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RBAC.RBAC.DAL.Models;

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
