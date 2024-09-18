using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RbacDashboard.DAL.Models;

[Table("OptionsetMaster", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class OptionsetMaster : EntityBase
{
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public Guid Value { get; set; }

    public string JsonObject { get; set; } = null!;

    [InverseProperty("OptionsetMaster")]
    [JsonIgnore]
    public virtual ICollection<Access> Accesses { get; set; } = new List<Access>();

    [InverseProperty("PermissionType")]
    public virtual ICollection<Permissionset> Permissionsets { get; set; } = new List<Permissionset>();

    [InverseProperty("OptionsetMaster")]
    public virtual ICollection<TypeMaster> TypeMasters { get; set; } = new List<TypeMaster>();
}