using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RBAC.RBAC.DAL.Models;

[Table("TypeMaster", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class TypeMaster : EntityBase
{
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public Guid? OptionsetMasterId { get; set; }

    [ForeignKey("OptionsetMasterId")]
    [InverseProperty("TypeMasters")]
    [JsonIgnore]
    public virtual OptionsetMaster? OptionsetMaster { get; set; }

    [InverseProperty("TypeMaster")]
    [JsonIgnore]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
