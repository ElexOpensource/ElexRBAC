using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Application;

/// <summary>
/// Query to get applications by customer ID.
/// </summary>
/// <param name="customerId">The ID of the customer.</param>
/// <param name="isActive">Indicates whether to filter by active applications.</param>
/// <param name="includingCustomer">Indicates whether to include customer details in the result.</param>
public class GetApplicationByCustomerId(Guid customerId, bool isActive = true, bool includingCustomer = false) : IRequest<List<RBAC.DAL.Models.Application>>
{
    public Guid CustomerId { get; } = customerId;

    public bool IsActive { get; } = isActive;

    public bool IsIncludeCustomer { get; } = includingCustomer;
}

public class GetApplicationsByCustomerIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetApplicationByCustomerId, List<RBAC.DAL.Models.Application>>(dbContext)
{
    public override async Task<List<RBAC.DAL.Models.Application>> Handle(GetApplicationByCustomerId request, CancellationToken cancellationToken)
    {
        IQueryable<RBAC.DAL.Models.Application> query = _dbContext.Applications
            .Where(app => app.CustomerId == request.CustomerId && !app.IsDeleted && app.IsActive == request.IsActive);

        if (request.IsIncludeCustomer)
            query = query.Include(app => app.Customer);

        var applications = await query.ToListAsync(cancellationToken);
        return applications;
    }
}
