
using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get Permissionsets.
/// </summary>
/// <param name="isActive">Indicates whether to filter by active permission set.</param>
public class GetPermissionsets(bool isActive = true) : IRequest<List<Permissionset>>
{
    public bool IsActive { get; } = isActive;
}

public class GetPermissionsetsSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetPermissionsets, List<Permissionset>>(dbContext)
{
    public override async Task<List<Permissionset>> Handle(GetPermissionsets request, CancellationToken cancellationToken)
    {
        IQueryable<Permissionset> query = _dbContext.Permissionsets
            .Where(permission => !permission.IsDeleted && permission.IsActive == request.IsActive);

        var permissions = await query.ToListAsync(cancellationToken);
        return permissions;
    }
}