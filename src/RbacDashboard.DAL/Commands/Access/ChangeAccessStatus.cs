using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Enum;

namespace RbacDashboard.DAL.Commands;

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