using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Command to add or update an Role record based on the Id
/// </summary>
/// <param name="role">The Role object to be added or updated</param>
public class AddorUpdateRole(Role role) : IRequest<Role>
{
    public Role Role { get; } = role;
}

public class AddorUpdateRoleSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateRole, Role>(dbContext)
{
    public override async Task<Role> Handle(AddorUpdateRole request, CancellationToken cancellationToken)
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