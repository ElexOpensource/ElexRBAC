using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get role by ID.
/// </summary>
/// <param name="id">The ID of the Role</param>
/// <param name="isActive">Indicates whether to filter by active applications</param>
public class GetRoleById(Guid id, bool isActive = true) : IRequest<Role>
{
    public Guid Id { get; } = id;

    public bool IsActive { get; } = isActive;
}

public class GetRoleByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetRoleById, Role>(dbContext)
{
    public override async Task<Role> Handle(GetRoleById request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles
            .Where(role => role.Id == request.Id && !role.IsDeleted && role.IsActive == request.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

#pragma warning disable CS8603 // Possible null reference return.
        return role;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
