using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;

namespace RbacDashboard.DAL.Commands;

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