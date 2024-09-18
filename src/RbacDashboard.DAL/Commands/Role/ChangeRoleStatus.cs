using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Enum;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Update role status based on the Id and Status
/// </summary>
/// <param name="id">Role Id</param>
/// <param name="status">Status</param>
public class ChangeRoleStatus(Guid id, RecordStatus status) : IRequest<bool>
{
    public Guid Id { get; } = id;

    public RecordStatus Status { get; } = status;

}

public class ChangeRoleStatusSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<ChangeRoleStatus, bool>(dbContext)
{
    public override async Task<bool> Handle(ChangeRoleStatus request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FindAsync(new object[] { request.Id }, cancellationToken);

        if (role == null)
        {
            return false;
        }

        switch (request.Status)
        {
            case RecordStatus.Active:
                role.IsActive = true;
                break;

            case RecordStatus.Inactive:
                role.IsActive = false;
                break;

            case RecordStatus.Delete:
                role.IsDeleted = true;
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}