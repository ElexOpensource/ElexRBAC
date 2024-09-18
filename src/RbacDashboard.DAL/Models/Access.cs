using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RbacDashboard.DAL.Models;

[Table("Access", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class Access : EntityBase
{
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public Guid OptionsetMasterId { get; set; }

    public Guid ApplicationId { get; set; }

    public string? MetaData { get; set; }

    public Guid? ParentId { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("Accesses")]
    public virtual Application? Application { get; set; } = null!;

    [InverseProperty("Parent")]
    public virtual ICollection<Access> InverseParent { get; set; } = new List<Access>();

    [ForeignKey("OptionsetMasterId")]
    [InverseProperty("Accesses")]
    public virtual OptionsetMaster? OptionsetMaster { get; set; } = null!;

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Access? Parent { get; set; }

    [InverseProperty("Access")]
    [JsonIgnore]
    public virtual ICollection<RoleAccess> RoleAccesses { get; set; } = new List<RoleAccess>();
}