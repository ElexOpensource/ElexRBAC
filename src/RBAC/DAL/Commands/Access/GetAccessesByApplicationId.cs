using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Access;

/// <summary>
/// Query to get access by Application ID.
/// </summary>
/// <param name="id">The ID of the Customer</param>
/// <param name="isActive">Indicates whether to filter by active accesses</param>
public class GetAccessesByApplicationId(Guid id, bool isActive = true, bool includeOptionsetMaster = false) : IRequest<List<RBAC.DAL.Models.Access>>
{
    public Guid ApplicationId { get; } = id;

    public bool IsActive { get; } = isActive;

    public bool IsIncludeTypeMaster { get; } = includeOptionsetMaster;

}

public class GetAccessesByApplicationIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetAccessesByApplicationId, List<RBAC.DAL.Models.Access>>(dbContext)
{
    public override async Task<List<RBAC.DAL.Models.Access>> Handle(GetAccessesByApplicationId request, CancellationToken cancellationToken)
    {
        IQueryable<RBAC.DAL.Models.Access> query = _dbContext.Accesses
            .Where(access => access.ApplicationId == request.ApplicationId && !access.IsDeleted && access.IsActive == request.IsActive);

        if (request.IsIncludeTypeMaster)
            query = query.Include(access => access.OptionsetMaster);

        var accesses = await query.ToListAsync(cancellationToken);
        return accesses;
    }
}
