using RbacDashboard.Common;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Models.Domain;

namespace RbacDashboard.BAL;

public class RoleAccessRepository(IMediatorService mediator) : IRbacRoleAccessRepository
{
    private readonly IMediatorService _mediator = mediator;

    public async Task<List<RoleAccess>> GetByRoleId(Guid applicationId)
    {
        var accesses = await _mediator.SendRequest(new GetRoleAccessesByRoleId(applicationId, true));
        return accesses;
    }

    public async Task AddRemoveAccess(Guid applicationId, AddRemoveAccessRequest accessRequest)
    {
        var roleId = accessRequest.RoleId;

        foreach (var accessId in accessRequest.AddAccess) {
            await ProcessAddAccess(roleId, accessId, applicationId);
        }

        foreach (var accessId in accessRequest.RemoveAccess) {
            await ProcessRemoveAccess(roleId, accessId);
        }
    }

    public async Task<RoleAccess> AddorUpdate(RoleAccess roleAccess)
    {
        roleAccess = await _mediator.SendRequest(new AddorUpdateRoleAccess(roleAccess));
        return roleAccess;
    }

    public async Task Delete(Guid roleAccessId)
    {
        await _mediator.SendRequest(new DeleteRoleAccess(roleAccessId));
    }

    private async Task ProcessAddAccess(Guid roleId, Guid accessId, Guid applicationId)
    {
        var roleAccess = await _mediator.SendRequest(new GetRoleAccessesByRoleAndAccessId(roleId, accessId));
        if (roleAccess == null)
        {
            roleAccess = new RoleAccess
            {
                RoleId = roleId,
                AccessId = accessId,
                ApplicationId = applicationId,
                IsDeleted = false,
                IsActive = true,
                CreatedOn = DateTime.Now
            };
        }
        else
        {
            roleAccess.IsDeleted = false;
            roleAccess.IsActive = true;
        }

        await _mediator.SendRequest(new AddorUpdateRoleAccess(roleAccess));
    }

    private async Task ProcessRemoveAccess(Guid roleId, Guid accessId)
    {
        var roleAccess = await _mediator.SendRequest(new GetRoleAccessesByRoleAndAccessId(roleId, accessId));
        if (roleAccess != null)
        {
            roleAccess.IsActive = false;
            await _mediator.SendRequest(new AddorUpdateRoleAccess(roleAccess));
        }
    }
}
