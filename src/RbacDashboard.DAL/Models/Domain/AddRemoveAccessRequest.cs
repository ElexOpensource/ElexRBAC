using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Models.Domain;

public class AddRemoveAccessRequest
{
    public Guid RoleId { get; set; }

    public List<Guid> AddAccess { get; set; } = [];

    public List<Guid> RemoveAccess { get; set; } = [];
}
