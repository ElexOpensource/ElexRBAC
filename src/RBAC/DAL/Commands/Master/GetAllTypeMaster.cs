using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;
using RBAC.RBAC.DAL.Models;

namespace RBAC.RBAC.DAL.Commands.Master;

/// <summary>
/// Query to get Type Masters.
/// </summary>
/// <param name="isActive">Indicates whether to filter by active typeMaster</param>
public class GetAllTypeMaster(bool isActive = true) : IRequest<List<TypeMaster>>
{
    public bool IsActive { get; } = isActive;
}

public class GetTypeMasterByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetAllTypeMaster, List<TypeMaster>>(dbContext)
{
    public override async Task<List<TypeMaster>> Handle(GetAllTypeMaster request, CancellationToken cancellationToken)
    {
        var types = await _dbContext.TypeMasters
            .Where(type => !type.IsDeleted && type.IsActive == request.IsActive)
            .ToListAsync(cancellationToken);

#pragma warning disable CS8603 // Possible null reference return.
        return types;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
