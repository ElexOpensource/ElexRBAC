using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get application by ID.
/// </summary>
/// <param name="applicationId">The ID of the customer.</param>
/// <param name="isActive">Indicates whether to filter by active applications.</param>
public class GetApplicationById(Guid applicationId, bool isActive = true) : IRequest<Application>
{
    public Guid Id { get; } = applicationId;

    public bool IsActive { get; } = isActive;
}

public class GetApplicationByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetApplicationById, Application?>(dbContext)
{
    public override async Task<Application?> Handle(GetApplicationById request, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications
            .Where(app => app.Id == request.Id && !app.IsDeleted && app.IsActive == request.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return application;
    }
}

[Obsolete]
[ExcludeFromCodeCoverage(Justification = "Sample implementation; no need to include in code coverage.")]
public class GetApplicationByIdHandlerPgSql(RbacSqlDbContext dbContext) : PgSqlRequestHandler<GetApplicationById, Application?>(dbContext)
{
    public override async Task<Application?> Handle(GetApplicationById request, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications.FindAsync(request.Id, cancellationToken);

        if (application == null || application.IsDeleted || application.IsActive != request.IsActive)
        {
            return null;
        }

        return application;
    }
}
