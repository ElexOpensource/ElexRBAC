using MediatR;
using RbacDashboard.DAL.Data;

namespace RbacDashboard.DAL.Base.RequestHandler;

public interface ISqlHandler { }

/// <summary>
/// Abstract base class for handling SEQUEL requests involving entities. 
/// This class supports all the database types supported by EF Core.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="dbContext">The database context.</param>
public abstract class SqlRequestHandler<TRequest, TResponse>(RbacSqlDbContext dbContext) : IRequestHandler<TRequest, TResponse>, ISqlHandler where TRequest : IRequest<TResponse>
{
    protected readonly RbacSqlDbContext _dbContext = dbContext;

    /// <summary>
    /// Handles the request.
    /// </summary>
    /// <param name="request">The request containing parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}