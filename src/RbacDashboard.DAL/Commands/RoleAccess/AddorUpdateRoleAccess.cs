using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Command to add or update an RoleAccess record based on the Id
/// </summary>
/// <param name="roleAccess">The RoleAccess object to be added or updated</param>
public class AddorUpdateRoleAccess (RoleAccess roleAccess) : IRequest<RoleAccess>
{
    public RoleAccess RoleAccess { get; } = roleAccess;
}

public class AddorUpdateRoleAccessSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateRoleAccess, RoleAccess>(dbContext)
{
    public override async Task<RoleAccess> Handle(AddorUpdateRoleAccess request, CancellationToken cancellationToken)
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