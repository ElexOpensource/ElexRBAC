using MediatR;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;


namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get applications by customer ID.
/// </summary>
/// <param name="customerId">The ID of the customer.</param>
/// <param name="isActive">Indicates whether to filter by active applications.</param>
/// <param name="includingCustomer">Indicates whether to include customer details in the result.</param>
public class GetApplicationByCustomerId(Guid customerId, bool isActive = true, bool includingCustomer = false) : IRequest<List<Application>>
{
    public Guid CustomerId { get; } = customerId;

    public bool IsActive { get; } = isActive;

    public bool IsIncludeCustomer { get; } = includingCustomer;
}

public class GetApplicationsByCustomerIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetApplicationByCustomerId, List<Application>>(dbContext)
{
    public override async Task<List<Application>> Handle(GetApplicationByCustomerId request, CancellationToken cancellationToken)
    {
        IQueryable<Application> query = _dbContext.Applications
            .Where(app => app.CustomerId == request.CustomerId && !app.IsDeleted && app.IsActive == request.IsActive);

        if (request.IsIncludeCustomer)
            query = query.Include(app => app.Customer);
        
        var applications = await query.ToListAsync(cancellationToken);
        return applications;
    }
}