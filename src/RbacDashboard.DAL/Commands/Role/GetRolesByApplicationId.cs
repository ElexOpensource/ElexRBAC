using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get roles by Application ID.
/// </summary>
/// <param name="applicationId">The ID of the Application</param>
/// <param name="isActive">Indicates whether to filter by active applications</param>
/// <param name="includeTypeMaster">Indicates whether include type master along with role</param>
public class GetRolesByApplicationId(Guid applicationId, bool isActive = true, bool includeTypeMaster = false) : IRequest<List<Role>>
{
    public Guid ApplicationId { get; } = applicationId;

    public bool IsActive { get; } = isActive;

    public bool IsIncludeTypeMaster { get; } = includeTypeMaster;

}

public class GetRolesByApplicationIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetRolesByApplicationId, List<Role>>(dbContext)
{
    public override async Task<List<Role>> Handle(GetRolesByApplicationId request, CancellationToken cancellationToken)
    {
        IQueryable<Role> query = _dbContext.Roles
            .Where(role => role.ApplicationId == request.ApplicationId && !role.IsDeleted && role.IsActive == request.IsActive);

        if (request.IsIncludeTypeMaster)
            query = query.Include(role => role.TypeMaster);

        var roles = await query.ToListAsync(cancellationToken);
        return roles;
    }
}