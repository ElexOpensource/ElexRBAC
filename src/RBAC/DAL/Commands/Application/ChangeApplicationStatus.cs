using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;
using RBAC.RBAC.DAL.Enum;

namespace RBAC.RBAC.DAL.Commands.Application;

/// <summary>
/// Update application status based on the Id and Status
/// </summary>
/// <param name="id">Application Id</param>
/// <param name="status">Status</param>
public class ChangeApplicationStatus(Guid id, RecordStatus status) : IRequest<bool>
{
    public Guid Id { get; } = id;

    public RecordStatus Status { get; } = status;

}

public class ChangeApplicationStatusSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<ChangeApplicationStatus, bool>(dbContext)
{
    public override async Task<bool> Handle(ChangeApplicationStatus request, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications.FindAsync(new object[] { request.Id }, cancellationToken);

        if (application == null)
        {
            return false;
        }

        switch (request.Status)
        {
            case RecordStatus.Active:
                application.IsActive = true;
                break;

            case RecordStatus.Inactive:
                application.IsActive = false;
                break;

            case RecordStatus.Delete:
                application.IsDeleted = true;
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
