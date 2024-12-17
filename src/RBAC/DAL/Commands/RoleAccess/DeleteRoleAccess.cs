using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.RoleAccess;

/// <summary>
/// Command to delete Role Access
/// </summary>
/// <param name="roleAccessId">RoleAccess Id</param>
public class DeleteRoleAccess(Guid roleAccessId) : IRequest<bool>
{
    public Guid Id { get; } = roleAccessId;
}

public class DeleteRoleAccessSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<DeleteRoleAccess, bool>(dbContext)
{
    public override async Task<bool> Handle(DeleteRoleAccess request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.RoleAccesses.FindAsync(new object[] { request.Id }, cancellationToken);

        if (role == null)
        {
            return false;
        }

        _dbContext.Remove(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
