using RbacDashboard.DAL.Models;
using RbacDashboard.Common;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Enum;

namespace RbacDashboard.BAL;

public class RoleRepository(IMediatorService mediator) : IRbacRoleRepository
{
    private readonly IMediatorService _mediator = mediator;

    public async Task<Role> AddorUpdate(Role role)
    {
        return await _mediator.SendRequest(new AddorUpdateRole(role));
    }

    public async Task Delete(Guid roleId)
    {
        await _mediator.SendRequest(new DeleteRole(roleId));
    }

    public async Task<Role> GetById(Guid roleId)
    {
        var role = await _mediator.SendRequest(new GetRoleById(roleId));

        if (role != null)
        {
            return role;
        }

        throw new KeyNotFoundException($"Role with id - {roleId} is not available");
    }

    public async Task<List<Role>> GetByApplicationId(Guid applicationId, bool isActive)
    {
        var roles = await _mediator.SendRequest(new GetRolesByApplicationId(applicationId, isActive, true));
        return roles;
    }

    public async Task ChangeStatus(Guid roleId, RecordStatus status)
    {
        var isStatusChanged = await _mediator.SendRequest(new ChangeRoleStatus(roleId, status));
        if (!isStatusChanged)
        {
            throw new KeyNotFoundException($"Role with id - {roleId} is not available");
        }
    }
}