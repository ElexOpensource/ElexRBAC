namespace RbacDashboard.DAL.Models.Domain;

public class ApplicationClaim
{
    public required string Type { get; set; }
    public required string Value { get; set; }
}

public partial class ApplicationAuthenticationState
{
    public bool IsAuthenticated { get; set; }

    public string Name { get; set; } = string.Empty;

    public IEnumerable<ApplicationClaim> Claims { get; set; } = [];
}