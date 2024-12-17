using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;
using System.Diagnostics.CodeAnalysis;

namespace RBAC.RBAC.DAL.Commands.Application;

/// <summary>
/// Query to get application by ID.
/// </summary>
/// <param name="applicationId">The ID of the customer.</param>
/// <param name="isActive">Indicates whether to filter by active applications.</param>
public class GetApplicationById(Guid applicationId, bool isActive = true) : IRequest<RBAC.DAL.Models.Application>
{
    public Guid Id { get; } = applicationId;

    public bool IsActive { get; } = isActive;
}

public class GetApplicationByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetApplicationById, RBAC.DAL.Models.Application?>(dbContext)
{
    public override async Task<RBAC.DAL.Models.Application?> Handle(GetApplicationById request, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications
            .Where(app => app.Id == request.Id && !app.IsDeleted && app.IsActive == request.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return application;
    }
}

[Obsolete]
[ExcludeFromCodeCoverage(Justification = "Sample implementation; no need to include in code coverage.")]
public class GetApplicationByIdHandlerPgSql(RbacSqlDbContext dbContext) : PgSqlRequestHandler<GetApplicationById, RBAC.DAL.Models.Application?>(dbContext)
{
    public override async Task<RBAC.DAL.Models.Application?> Handle(GetApplicationById request, CancellationToken cancellationToken)
    {
        var application = await _dbContext.Applications.FindAsync(request.Id, cancellationToken);

        if (application == null || application.IsDeleted || application.IsActive != request.IsActive)
        {
            return null;
        }

        return application;
    }
}

