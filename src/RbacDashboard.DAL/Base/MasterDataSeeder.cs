using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Base;

/// <summary>
/// Provides methods to seed master data into the database.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]

internal static class MasterDataSeeder
{
    /// <summary>
    /// Seeds master data into the database. If the data already exists, it updates the existing records.
    /// </summary>
    /// <param name="context">The database context.</param>
    public static void SeedMasterData(DbContext context)
    {
        SeedData(context, RbacMasterData.OptionsetMasters, (existing, newData) =>
        {
            existing.IsActive = newData.IsActive;
            existing.IsDeleted = newData.IsDeleted;
            existing.Name = newData.Name;
            existing.Value = newData.Value;
            existing.JsonObject = newData.JsonObject;
        });

        SeedData(context, RbacMasterData.TypeMasters, (existing, newData) =>
        {
            existing.IsActive = newData.IsActive;
            existing.IsDeleted = newData.IsDeleted;
            existing.Name = newData.Name;
            existing.OptionsetMasterId = newData.OptionsetMasterId;
        });

        SeedData(context, RbacMasterData.Permissionsets, (existing, newData) =>
        {
            existing.IsActive = newData.IsActive;
            existing.IsDeleted = newData.IsDeleted;
            existing.Name = newData.Name;
            existing.PermissionTypeId = newData.PermissionTypeId;
            existing.ParentId = newData.ParentId;
        });

        context.SaveChanges();
    }

    /// <summary>
    /// Seeds a list of data into the database. If an item already exists, it updates the existing record.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="context">The database context.</param>
    /// <param name="dataList">The list of data to seed.</param>
    /// <param name="updateExisting">An action that updates the existing data with new data.</param>
    private static void SeedData<T>(DbContext context, IEnumerable<T> dataList, Action<T, T> updateExisting) where T : class
    {
        foreach (var data in dataList)
        {
            var existingData = context.Set<T>().Find(((dynamic)data).Id);
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
