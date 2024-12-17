using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;
using RBAC.RBAC.DAL.Enum;

namespace RBAC.RBAC.DAL.Commands.Access;

/// <summary>
/// Update access status based on the Id and Status
/// </summary>
/// <param name="id">Access Id</param>
/// <param name="status">Status</param>
public class ChangeAccessStatus(Guid id, RecordStatus status) : IRequest<bool>
{
    public Guid Id { get; } = id;

    public RecordStatus Status { get; } = status;

}

public class ChangeAccessStatusSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<ChangeAccessStatus, bool>(dbContext)
{
    public override async Task<bool> Handle(ChangeAccessStatus request, CancellationToken cancellationToken)
    {
        var access = await _dbContext.Accesses.FindAsync(new object[] { request.Id }, cancellationToken);

        if (access == null)
        {
            return false;
        }

        switch (request.Status)
        {
            case RecordStatus.Active:
                access.IsActive = true;
                break;

            case RecordStatus.Inactive:
                access.IsActive = false;
                break;

            case RecordStatus.Delete:
                access.IsDeleted = true;
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
