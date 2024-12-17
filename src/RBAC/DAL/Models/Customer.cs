using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RBAC.RBAC.DAL.Models;

[Table("Customer", Schema = "RBAC")]
[ExcludeFromCodeCoverage(Justification = "Models do not need to be included in code coverage.")]
public partial class Customer : EntityBase
{
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Customer")]
    [JsonIgnore]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}
