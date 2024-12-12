using MediatR;
using Microsoft.EntityFrameworkCore;
using RBAC.RBAC.DAL.Base.RequestHandler;
using RBAC.RBAC.DAL.Context;
using RBAC.RBAC.DAL.Enum;
using RBAC.RBAC.DAL.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RBAC.RBAC.DAL.Commands;

public class DataMigration(string masterData, RbacTable dbTable) : IRequest<bool>
{
    public string MasterData { get; } = masterData;

    public RbacTable DbTable { get; } = dbTable;
}


[ExcludeFromCodeCoverage(Justification = "Transaction can't be included in the in-memory DB")]
public class DataMigrationSqlHandler(RbacSqlDbContext dbContext) : SqlRequestHandler<DataMigration, bool>(dbContext)
{
    public override Task<bool> Handle(DataMigration request, CancellationToken cancellationToken)
    {
        using var transaction = this._dbContext.Database.BeginTransaction();
        try
        {
            switch (request.DbTable)
            {
                case RbacTable.Customer:
                    var customer = JsonSerializer.Deserialize<List<Customer>>(request.MasterData);
                    SeedData(this._dbContext, customer!, (existing, newData) =>
                    {
                        existing.IsActive = newData.IsActive;
                        existing.IsDeleted = newData.IsDeleted;
                        existing.Name = newData.Name;
                        existing.CreatedOn = newData.CreatedOn;
                    });
                    break;

                case RbacTable.Application:
                    var application = JsonSerializer.Deserialize<List <RBAC.DAL.Models.Application>> (request.MasterData);
                    SeedData(this._dbContext, application!, (existing, newData) =>
                    {
                        existing.IsActive = newData.IsActive;
                        existing.IsDeleted = newData.IsDeleted;
                        existing.Name = newData.Name;
                        existing.CustomerId = newData.CustomerId;
                        existing.CreatedOn = newData.CreatedOn;
                    });
                    break;

                case RbacTable.Role:
                    var role = JsonSerializer.Deserialize<List<RBAC.DAL.Models.Role>>(request.MasterData);
                    SeedData(this._dbContext, role!, (existing, newData) =>
                    {
                        existing.IsActive = newData.IsActive;
                        existing.IsDeleted = newData.IsDeleted;
                        existing.CreatedOn = newData.CreatedOn;
                        existing.Name = newData.Name;
                        existing.ApplicationId = newData.ApplicationId;
                        existing.TypeMasterId = newData.TypeMasterId;
                        existing.ParentId = newData.ParentId;
                    });
                    break;

                case RbacTable.Access:
                    var access = JsonSerializer.Deserialize<List<RBAC.DAL.Models.Access>>(request.MasterData);
                    SeedData(this._dbContext, access!, (existing, newData) =>
                    {
                        existing.IsActive = newData.IsActive;
                        existing.IsDeleted = newData.IsDeleted;
                        existing.CreatedOn = newData.CreatedOn;
                        existing.Name = newData.Name;
                        existing.ApplicationId = newData.ApplicationId;
                        existing.OptionsetMasterId = newData.OptionsetMasterId;
                        existing.MetaData = newData.MetaData;
                        existing.ParentId = newData.ParentId;
                        ;
                    });
                    break;

                case RbacTable.RoleAccess:
                    var roleAccess = JsonSerializer.Deserialize<List<RBAC.DAL.Models.RoleAccess>>(request.MasterData);
                    SeedData(this._dbContext, roleAccess!, (existing, newData) =>
                    {
                        existing.IsActive = newData.IsActive;
                        existing.IsDeleted = newData.IsDeleted;
                        existing.CreatedOn = newData.CreatedOn;
                        existing.RoleId = newData.RoleId;
                        existing.AccessId = newData.AccessId;
                        existing.AccessMetaData = newData.AccessMetaData;
                        existing.ApplicationId = newData.ApplicationId;
                        existing.IsOverwrite = newData.IsOverwrite;
                        ;
                    });
                    break;

                default:
                    throw new InvalidDataException($"Db type not found - {request.DbTable.ToString()}");
            }

            this._dbContext.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        return Task.FromResult(true);
    }

    private static void SeedData<T>(DbContext context, IEnumerable<T> dataList, Action<T, T> updateExisting) where T : EntityBase
    {
        var ids = dataList.Select(data => data.Id).ToList();
        var existingDataList = context.Set<T>().Where(e => ids.Contains(e.Id)).ToList();

        foreach (var data in dataList)
        {
            var existingData = existingDataList.SingleOrDefault(e => e.Id == data.Id);
            if (existingData == null)
            {
                context.Set<T>().Add(data);
            }
            else
            {
                updateExisting(existingData, data);
            }
        }
    }
}
