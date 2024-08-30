using RbacDashboard.DAL.Models;
using RbacDashboard.Common;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Commands;

namespace RbacDashboard.BAL;

public class RoleRepository(IMediatorService mediator) : IRbacRoleRepository
{
    private readonly IMediatorService _mediator = mediator;

    public async Task<Role> AddorUpdate(Role role)
    {
        return await _mediator.SendRequest(new AddorUpdateRole(role));
    }

    public async Task Delete(Guid applicationId)
    {
        await _mediator.SendRequest(new DeleteRole(applicationId));
    }

    public async Task<Role> GetById(Guid applicationId)
    {
        var role = await _mediator.SendRequest(new GetRoleById(applicationId));

        if (role != null)
        {
            return role;
        }

        throw new KeyNotFoundException($"Role with id - {applicationId} is not available");
    }

    public async Task<List<Role>> GetByApplicationId(Guid applicationId)
    {
        var roles = await _mediator.SendRequest(new GetRolesByApplicationId(applicationId, true, true));
        return roles;
    }
}