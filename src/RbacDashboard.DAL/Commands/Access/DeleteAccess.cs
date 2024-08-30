using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Command to delete Access
/// </summary>
/// <param name="accessId">Access Id</param>
public class DeleteAccess(Guid accessId) : IRequest<bool>
{
    public Guid Id { get; } = accessId;
}

public class DeleteAccessSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<DeleteAccess, bool>(dbContext)
{
    public override async Task<bool> Handle(DeleteAccess request, CancellationToken cancellationToken)
    {
        var access = await _dbContext.Accesses.FindAsync(new object[] { request.Id }, cancellationToken);

        if (access == null)
        {
            return false;
        }

        _dbContext.Remove(access);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
