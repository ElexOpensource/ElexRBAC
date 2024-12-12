using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;
using RBAC.RBAC.DAL.Models;

namespace RBAC.RBAC.DAL.Commands.Master;

/// <summary>
/// Query to get OptionsetMasters.
/// </summary>
/// <param name="isActive">Indicates whether to filter by active OptionsetMasters.</param>
public class GetOptionsetMasters(bool isActive = true) : IRequest<List<OptionsetMaster>>
{
    public bool IsActive { get; } = isActive;
}

public class GetOptionsetMastersSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetOptionsetMasters, List<OptionsetMaster>>(dbContext)
{
    public override async Task<List<OptionsetMaster>> Handle(GetOptionsetMasters request, CancellationToken cancellationToken)
    {
        IQueryable<OptionsetMaster> query = _dbContext.OptionsetMasters
            .Where(permission => !permission.IsDeleted && permission.IsActive == request.IsActive);

        var options = await query.ToListAsync(cancellationToken);
        return options;
    }
}
