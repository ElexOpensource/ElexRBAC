using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Role;

/// <summary>
/// Query to get role accesses by roles
/// </summary>
/// <param name="roleList">List of role Ids</param>
public class GetRoleAccessByRoleIds(List<Guid> roleList) : IRequest<List<RBAC.DAL.Models.RoleAccess>>
{
    public IList<Guid> RoleList { get; } = roleList;
}

public class GetAccessMetadataByRoleIdsSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetRoleAccessByRoleIds, List<RBAC.DAL.Models.RoleAccess>>(dbContext)
{
    public override async Task<List<RBAC.DAL.Models.RoleAccess>> Handle(GetRoleAccessByRoleIds request, CancellationToken cancellationToken)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var roleAccesses = await _dbContext.RoleAccesses
                .Include(roleAccess => roleAccess.Role)
                .Include(roleAccess => roleAccess.Access)
                .Where(roleAccess => request.RoleList.Contains(roleAccess.RoleId))
                .Where(roleAccess => roleAccess.IsActive == true && roleAccess.IsDeleted != true &&
                    roleAccess.Role.IsActive && !roleAccess.Role.IsDeleted &&
                    roleAccess.Access.IsActive && !roleAccess.Access.IsDeleted)
                .ToListAsync(cancellationToken: cancellationToken);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        return roleAccesses;
    }
}
