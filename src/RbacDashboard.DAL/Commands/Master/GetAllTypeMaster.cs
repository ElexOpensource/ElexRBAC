using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

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
