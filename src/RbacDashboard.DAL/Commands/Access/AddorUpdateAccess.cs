using MediatR;
using RbacDashboard.DAL.Base.RequestHandler;
using RbacDashboard.DAL.Data;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.DAL.Commands;

/// <summary>
/// Command to add or update an Access record based on the Id
/// </summary>
/// <param name="access">The Access object to be added or updated</param>
public class AddorUpdateAccess(Access access) : IRequest<Access>
{
    public Access Access { get; } = access;
}

public class AddorUpdateAccessSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateAccess, Access>(dbContext)
{
    public override async Task<Access> Handle(AddorUpdateAccess request, CancellationToken cancellationToken)
    {
        var access = request.Access;
        if (access.Id != Guid.Empty)
        {
            _dbContext.Accesses.Update(access);
        }
        else
        {
            await _dbContext.Accesses.AddAsync(access, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return access;
    }
}