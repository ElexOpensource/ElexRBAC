using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.RoleAccess;

/// <summary>
/// Query to get role accesses by Role and Access Id.
/// </summary>
/// <param name="roleId">The ID of the Role</param>
/// <param name="accessId">The ID of the Access</param>
/// <param name="filterActiveOnly">Indicates whether to filter by active only</param>
public class GetRoleAccessesByRoleAndAccessId(Guid roleId, Guid accessId, bool filterActiveOnly = false) : IRequest<RBAC.DAL.Models.RoleAccess>
{
    public Guid RoleId { get; } = roleId;

    public Guid AccessId { get; } = accessId;

    public bool IsActive { get; } = filterActiveOnly;
}

public class GetRoleAccessesByRoleAndAccessSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetRoleAccessesByRoleAndAccessId, RBAC.DAL.Models.RoleAccess>(dbContext)
{
    public override async Task<RBAC.DAL.Models.RoleAccess> Handle(GetRoleAccessesByRoleAndAccessId request, CancellationToken cancellationToken)
    {
        IQueryable<RBAC.DAL.Models.RoleAccess> query = _dbContext.RoleAccesses
            .Where(roleAccess => roleAccess.RoleId == request.RoleId && roleAccess.AccessId == request.AccessId && !roleAccess.IsDeleted);

        if (request.IsActive)
            query = query.Where(roleAccess => roleAccess.IsActive);

        var roleAccesses = await query.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

#pragma warning disable CS8603 // Possible null reference return.
        return roleAccesses;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
