using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Application;

/// <summary>
/// Command to add or update an Application record based on the Id
/// </summary>
/// <param name="application">The Application object to be added or updated</param>
public class AddorUpdateApplication(RBAC.DAL.Models.Application application) : IRequest<RBAC.DAL.Models.Application>
{
    public RBAC.DAL.Models.Application Application { get; } = application;
}

public class AddorUpdateApplicationsSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateApplication, RBAC.DAL.Models.Application>(dbContext)
{
    public override async Task<RBAC.DAL.Models.Application> Handle(AddorUpdateApplication request, CancellationToken cancellationToken)
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
