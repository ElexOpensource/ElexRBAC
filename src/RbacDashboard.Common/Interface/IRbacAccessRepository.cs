﻿using RbacDashboard.DAL.Models;

namespace RbacDashboard.Common.Interface;

public interface IRbacAccessRepository
{
    Task<List<Access>> GetByApplicationId(Guid applicationId);

    Task<Access> GetById(Guid accessId);

    Task<Access> AddorUpdate(Access access);

    Task Delete(Guid accessId);
}