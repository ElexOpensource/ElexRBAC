using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Enum;

namespace RbacDashboard.DAL.Commands;

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
