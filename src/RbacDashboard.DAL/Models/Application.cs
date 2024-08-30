using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RbacDashboard.DAL.Models;

[Table("Application", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class Application
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [StringLength(100)]
    public string ApplicationName { get; set; } = null!;

    public Guid CustomerId { get; set; }

    [InverseProperty("Application")]
    public virtual ICollection<Access> Accesses { get; set; } = new List<Access>();

    [ForeignKey("CustomerId")]
    [InverseProperty("Applications")]
    public virtual Customer? Customer { get; set; } = null!;

    [InverseProperty("Application")]
    public virtual ICollection<RoleAccess> RoleAccesses { get; set; } = new List<RoleAccess>();

    [InverseProperty("Application")]
    [JsonIgnore]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}