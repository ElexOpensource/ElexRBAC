using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Role;

/// <summary>
/// Command to delete Role
/// </summary>
/// <param name="roleId">Role Id</param>
public class DeleteRole(Guid roleId) : IRequest<bool>
{
    public Guid Id { get; } = roleId;
}

public class DeleteRoleSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<DeleteRole, bool>(dbContext)
{
    public override async Task<bool> Handle(DeleteRole request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FindAsync(new object[] { request.Id }, cancellationToken);

        if (role == null)
        {
            return false;
        }

        _dbContext.Remove(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
