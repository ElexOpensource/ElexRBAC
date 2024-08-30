using MediatR;
using RbacDashboard.DAL.Models;
using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Base.RequestHandler;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Query to get customer by ID.
/// </summary>
/// <param name="customerId">The ID of the customer.</param>
/// <param name="isActive">Indicates whether to filter by active customer.</param>
public class GetCustomerById(Guid customerId, bool isActive = true) : IRequest<Customer>
{
    public Guid Id { get; } = customerId;

    public bool IsActive { get; } = isActive;
}

public class GetCustomerByIdSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<GetCustomerById, Customer?>(dbContext)
{
    public override async Task<Customer?> Handle(GetCustomerById request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
            .Where(app => app.Id == request.Id && !app.IsDeleted && app.IsActive == request.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return customer;
    }
}

[Obsolete]
[ExcludeFromCodeCoverage(Justification = "Sample implementation; no need to include in code coverage.")]
public class GetCustomerByIdHandlerPgSql(RbacSqlDbContext dbContext) : PgSqlRequestHandler<GetCustomerById, Customer?>(dbContext)
{
    public override async Task<Customer?> Handle(GetCustomerById request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.FindAsync(request.Id, cancellationToken);

        if (customer == null || customer.IsDeleted || customer.IsActive != request.IsActive)
        {
            return null;
        }

        return customer;
    }
}
