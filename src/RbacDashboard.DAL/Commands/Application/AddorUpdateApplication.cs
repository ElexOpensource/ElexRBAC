using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Command to add or update an Application record based on the Id
/// </summary>
/// <param name="application">The Application object to be added or updated</param>
public class AddorUpdateApplication(Application application) : IRequest<Application>
{
    public Application Application { get; } = application;
}

public class AddorUpdateApplicationsSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateApplication, Application>(dbContext)
{
    public override async Task<Application> Handle(AddorUpdateApplication request, CancellationToken cancellationToken)
    {
        var application = request.Application;
        if (application.Id != Guid.Empty)
        {
            _dbContext.Applications.Update(application);
        }
        else
        {
            await _dbContext.Applications.AddAsync(application, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return application;
    }
}