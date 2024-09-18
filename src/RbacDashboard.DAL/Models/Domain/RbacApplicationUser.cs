namespace RbacDashboard.DAL.Models.Domain;

public class RbacApplicationUser
{
    public required string CustomerId { get; set; }

    public required string CustomerName { get; set; }

    public string? ApplicationId { get; set; }

    public string? ApplicationName { get; set; }
}