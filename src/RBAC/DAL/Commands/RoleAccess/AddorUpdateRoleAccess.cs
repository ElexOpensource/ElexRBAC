using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.RoleAccess;

/// <summary>
/// Command to add or update an RoleAccess record based on the Id
/// </summary>
/// <param name="roleAccess">The RoleAccess object to be added or updated</param>
public class AddorUpdateRoleAccess(RBAC.DAL.Models.RoleAccess roleAccess) : IRequest<RBAC.DAL.Models.RoleAccess>
{
    public RBAC.DAL.Models.RoleAccess RoleAccess { get; } = roleAccess;
}

public class AddorUpdateRoleAccessSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateRoleAccess, RBAC.DAL.Models.RoleAccess>(dbContext)
{
    public override async Task<RBAC.DAL.Models.RoleAccess> Handle(AddorUpdateRoleAccess request, CancellationToken cancellationToken)
    {
        var roleAccess = request.RoleAccess;
        if (roleAccess.Id != Guid.Empty)
        {
            _dbContext.RoleAccesses.Update(roleAccess);
        }
        else
        {
            await _dbContext.RoleAccesses.AddAsync(roleAccess, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return roleAccess;
    }
}
