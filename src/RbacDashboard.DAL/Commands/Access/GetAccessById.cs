using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get access by ID.
/// </summary>
/// <param name="id">The ID of the Application</param>
/// <param name="isActive">Indicates whether to filter by active applications</param>
public class GetAccessById(Guid id, bool isActive = true) : IRequest<Access>
{
    public Guid Id { get; } = id;

    public bool IsActive { get; } = isActive;
}

public class GetAccessByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetAccessById, Access>(dbContext)
{
    public override async Task<Access> Handle(GetAccessById request, CancellationToken cancellationToken)
    {
        var access = await _dbContext.Accesses
            .Where(access => access.Id == request.Id && !access.IsDeleted && access.IsActive == request.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

#pragma warning disable CS8603 // Possible null reference return.
        return access;
#pragma warning restore CS8603 // Possible null reference return.
    }
}