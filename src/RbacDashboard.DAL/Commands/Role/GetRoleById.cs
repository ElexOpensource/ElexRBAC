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
public class GetRoleById(Guid id, bool isActive = true, bool includeStatusCheck = true) : IRequest<Role>
{
    public Guid Id { get; } = id;

    public bool IsActive { get; } = isActive;

    public bool IncludeStatusCheck { get; } = includeStatusCheck;
}

public class GetRoleByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetRoleById, Role>(dbContext)
{
    public override async Task<Role> Handle(GetRoleById request, CancellationToken cancellationToken)
    {
        IQueryable<Role> query = _dbContext.Roles
            .Where(role => role.Id == request.Id && !role.IsDeleted);

        if (request.IncludeStatusCheck)
            query = query.Where(role => role.IsActive == request.IsActive);

        var role = await query.FirstOrDefaultAsync(cancellationToken);

#pragma warning disable CS8603 // Possible null reference return.
        return role;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
