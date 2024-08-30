using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get roles by Role ID.
/// </summary>
/// <param name="roleId">The ID of the Role</param>
/// <param name="isActive">Indicates whether to filter by active RoleAccess</param>
/// <param name="includeRole">Indicates whether include Role along with RoleAccess</param>
/// <param name="includeAccess">Indicates whether include Access along with RoleAccess</param>
public class GetRoleAccessesByRoleId(Guid roleId,  bool isActive = true, bool includeRole = true, bool includeAccess = true) : IRequest<List<RoleAccess>>
{
    public Guid RoleId { get; } = roleId;

    public bool IsActive { get; } = isActive;

    public bool IncludeRole { get; } = includeRole;

    public bool IncludeAccess { get; } = includeAccess;
}

public class GetRoleAccessesByRoleIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetRoleAccessesByRoleId, List<RoleAccess>>(dbContext)
{
    public override async Task<List<RoleAccess>> Handle(GetRoleAccessesByRoleId request, CancellationToken cancellationToken)
    {
        IQueryable<RoleAccess> query = _dbContext.RoleAccesses
            .Where(roleAccess => roleAccess.RoleId == request.RoleId && !roleAccess.IsDeleted && roleAccess.IsActive == request.IsActive);

        if (request.IncludeRole)
            query = query.Include(roleAccess => roleAccess.Role)
                         .Include(roleAccess => roleAccess.Role!.TypeMaster);

        if (request.IncludeAccess)
            query = query.Include(roleAccess => roleAccess.Access)
                         .Include(roleAccess => roleAccess.Access!.OptionsetMaster);

        var roleAccesses = await query.AsNoTracking().ToListAsync(cancellationToken);
        return roleAccesses;
    }
}