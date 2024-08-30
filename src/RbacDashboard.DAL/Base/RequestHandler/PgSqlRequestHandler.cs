using MediatR;
using RbacDashboard.DAL.Data;
using System.Diagnostics.CodeAnalysis;


namespace RbacDashboard.DAL.Base.RequestHandler
{
    [Obsolete("This was created for a POC to register CQRS commands based on the DB type. Use this as an example for new DB implementations. For all DB types supported by EF, use ISqlHandler.")]
    public interface IPgSqlHandler { }

    [Obsolete]
    [ExcludeFromCodeCoverage(Justification = "Sample implementation; no need to include in code coverage.")]
    public abstract class PgSqlRequestHandler<TRequest, TResponse>(RbacSqlDbContext dbContext) : IRequestHandler<TRequest, TResponse>, IPgSqlHandler where TRequest : IRequest<TResponse>
    {
        protected readonly RbacSqlDbContext _dbContext = dbContext;

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}