using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Role;

/// <summary>
/// Command to add or update an Role record based on the Id
/// </summary>
/// <param name="role">The Role object to be added or updated</param>
public class AddorUpdateRole(RBAC.DAL.Models.Role role) : IRequest<RBAC.DAL.Models.Role>
{
    public RBAC.DAL.Models.Role Role { get; } = role;
}

public class AddorUpdateRoleSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateRole, RBAC.DAL.Models.Role>(dbContext)
{
    public override async Task<RBAC.DAL.Models.Role> Handle(AddorUpdateRole request, CancellationToken cancellationToken)
    {
        var role = request.Role;
        if (role.Id != Guid.Empty)
        {
            _dbContext.Roles.Update(role);
        }
        else
        {
            await _dbContext.Roles.AddAsync(role, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return role;
    }
}
