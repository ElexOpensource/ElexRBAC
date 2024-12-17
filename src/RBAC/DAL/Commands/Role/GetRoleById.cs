using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Role;

/// <summary>
/// Query to get role by ID.
/// </summary>
/// <param name="id">The ID of the Role</param>
/// <param name="isActive">Indicates whether to filter by active applications</param>
public class GetRoleById(Guid id, bool isActive = true, bool includeStatusCheck = true) : IRequest<RBAC.DAL.Models.Role>
{
    public Guid Id { get; } = id;

    public bool IsActive { get; } = isActive;

    public bool IncludeStatusCheck { get; } = includeStatusCheck;
}

public class GetRoleByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetRoleById, RBAC.DAL.Models.Role>(dbContext)
{
    public override async Task<RBAC.DAL.Models.Role> Handle(GetRoleById request, CancellationToken cancellationToken)
    {
        IQueryable<RBAC.DAL.Models.Role> query = _dbContext.Roles
            .Where(role => role.Id == request.Id && !role.IsDeleted);

        if (request.IncludeStatusCheck)
            query = query.Where(role => role.IsActive == request.IsActive);

        var role = await query.FirstOrDefaultAsync(cancellationToken);

#pragma warning disable CS8603 // Possible null reference return.
        return role;
#pragma warning restore CS8603 // Possible null reference return.
    }
}

