using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Command to delete Application
/// </summary>
/// <param name="ApplicationId">Application Id</param>
public class DeleteApplication(Guid ApplicationId) : IRequest<bool>
{
    public Guid Id { get; } = ApplicationId;
}

public class DeleteApplicationSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<DeleteApplication, bool>(dbContext)
{
    public override async Task<bool> Handle(DeleteApplication request, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications.FindAsync(new object[] { request.Id }, cancellationToken);

        if (application == null)
        {
            return false;
        }

        _dbContext.Remove(application);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}