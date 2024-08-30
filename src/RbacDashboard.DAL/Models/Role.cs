using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RbacDashboard.DAL.Models;

[Table("Role", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class Role
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    public Guid ApplicationId { get; set; }

    public Guid TypeMasterId { get; set; }

    public Guid? ParentId { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("Roles")]
    public virtual Application? Application { get; set; } = null!;

    [InverseProperty("Parent")]
    [JsonIgnore]
    public virtual ICollection<Role> InverseParent { get; set; } = new List<Role>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Role? Parent { get; set; }

    [InverseProperty("Role")]
    [JsonIgnore]
    public virtual ICollection<RoleAccess> RoleAccesses { get; set; } = new List<RoleAccess>();

    [ForeignKey("TypeMasterId")]
    [InverseProperty("Roles")]
    public virtual TypeMaster? TypeMaster { get; set; } = null!;
}