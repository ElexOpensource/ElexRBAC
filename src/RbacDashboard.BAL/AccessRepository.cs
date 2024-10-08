﻿using RbacDashboard.Common;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Commands;
using RbacDashboard.DAL.Enum;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.BAL;

public class AccessRepository(IMediatorService mediator) : IRbacAccessRepository
{
    private readonly IMediatorService _mediator = mediator;

    public async Task<Access> AddorUpdate(Access access)
    {
        return await _mediator.SendRequest(new AddorUpdateAccess(access));
    }

    public async Task Delete(Guid accessId)
    {
        await _mediator.SendRequest(new DeleteAccess(accessId));
    }

    public async Task ChangeStatus(Guid accessId, RecordStatus status)
    {
        var isStatusChanged =  await _mediator.SendRequest(new ChangeAccessStatus(accessId, status));
        if (!isStatusChanged)
        {
            throw new KeyNotFoundException($"Access with id - {accessId} is not available");
        }
    }

    public async Task<Access> GetById(Guid accessId)
    {
        var access = await _mediator.SendRequest(new GetAccessById(accessId));

        if (access != null)
        {
            return access;
        }

        throw new KeyNotFoundException($"Access with id - {accessId} is not available");
    }

    public async Task<List<Access>> GetByApplicationId(Guid applicationId, bool isActive)
    {
        var accesses = await _mediator.SendRequest(new GetAccessesByApplicationId(applicationId, isActive, true));
        return accesses;
    }

}
