using MediatR;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;

namespace RBAC.RBAC.DAL.Commands.Access;

/// <summary>
/// Command to add or update an Access record based on the Id
/// </summary>
/// <param name="access">The Access object to be added or updated</param>
public class AddorUpdateAccess(RBAC.DAL.Models.Access access) : IRequest<RBAC.DAL.Models.Access>
{
    public RBAC.DAL.Models.Access Access { get; } = access;
}

public class AddorUpdateAccessSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<AddorUpdateAccess, RBAC.DAL.Models.Access>(dbContext)
{
    public override async Task<RBAC.DAL.Models.Access> Handle(AddorUpdateAccess request, CancellationToken cancellationToken)
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
