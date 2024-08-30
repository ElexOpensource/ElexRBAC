using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RbacDashboard.DAL.Models;

[Table("Customer", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class Customer
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [StringLength(100)]
    public string CustomerName { get; set; } = null!;

    [InverseProperty("Customer")]
    [JsonIgnore]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}