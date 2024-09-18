using Newtonsoft.Json;
using RbacDashboard.Common;
using RbacDashboard.DAL.Commands;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Models.Domain;
using RbacDashboard.DAL.Models;

namespace RbacDashboard.BAL;

public class AccessTokenRepository(IMediatorService mediator, IRbacTokenRepository tokenRepository) : IRbacAccessTokenRepository
{
    private readonly IMediatorService _mediator = mediator;

    private readonly IRbacTokenRepository _token = tokenRepository;

    public async Task<string> GetByRoleIds(List<Guid> roleIds)
    {
        var allRoleIds = new HashSet<Guid>();

        var role = await _mediator.SendRequest(new GetRoleById(roleIds.First(), includeStatusCheck: false));
        var allRoles = await _mediator.SendRequest(new GetRolesByApplicationId(role.ApplicationId, true, false));

        var parentIds = GetParentRolesForRoleId(roleIds, allRoles);
        var childIds = GetChildRolesForRoleId(roleIds, allRoles);

        var ParentRoleAccesses = await _mediator.SendRequest(new GetRoleAccessByRoleIds([.. parentIds]));
        var childRoleAccesses = await _mediator.SendRequest(new GetRoleAccessByRoleIds([.. childIds]));

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
        var parentAccessMetaData = ParentRoleAccesses
               .Select(roleAccess => new
               {
                   AccessId = roleAccess.AccessId,
                   AccessName = roleAccess?.Access?.AccessName,
                   AccessJson = JsonConvert.DeserializeObject<AccessJSON>(roleAccess?.Access.MetaData ?? "")
               })
               .Where(item => item.AccessJson != null && item.AccessJson.CanInherit == true)
               .Select(item => new AccessMetaData
               {
                   AccessId = item.AccessId,
                   AccessName = item.AccessName!,
                   Permissions = item.AccessJson!.Permissions,
               }).ToList();

        var chilsdAccessMetaData = childRoleAccesses
                .Select(roleAccess => new
                {
                    AccessId = roleAccess.AccessId,
                    AccessName = roleAccess?.Access?.AccessName,
                    AccessJson = JsonConvert.DeserializeObject<AccessJSON>(roleAccess.IsOverwrite ? roleAccess.AccessMetaData : roleAccess.Access.MetaData ?? "")
                })
                .Where(item => item.AccessJson != null)
                .Select(item => new AccessMetaData
                {
                    AccessId = item.AccessId,
                    AccessName = item.AccessName!,
                    Permissions = item.AccessJson!.Permissions,
                })
                .ToList();


        var accessMetaData = new List<AccessMetaData>(parentAccessMetaData);
        accessMetaData.AddRange(chilsdAccessMetaData);

        var metaData = accessMetaData
                    .GroupBy(item => item.AccessId)
                    .Select(group => new AccessMetaData
                    {
                        AccessId = group.Key,
                        AccessName = group.First().AccessName,
                        Permissions = group
                                        .SelectMany(item => item.Permissions)
                                        .GroupBy(p => p.Id)
                                        .Select(pGroup => pGroup.First())
                                        .ToList()
                    }).ToList();

#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (accessMetaData.Count == 0)
        {
            return string.Empty;
        }

        return _token.GenerateJwtToken(JsonConvert.SerializeObject(accessMetaData));
    }

    private static List<Guid> GetChildRolesForRoleId(List<Guid> roleIds, List<Role> allRoles)
    {
        var childRoleIds = new List<Guid>();

        if (allRoles.Count < 1) { return childRoleIds; }

        foreach (var roleId in roleIds)
        {
            if (childRoleIds.Contains(roleId)) { continue; }

            childRoleIds.Add(roleId);
            GetChildRoleIdsInMemory(roleId, allRoles, childRoleIds);
        }

        return childRoleIds;
    }

    private static List<Guid> GetParentRolesForRoleId(List<Guid> roleIds, List<Role> allRoles)
    {
        var parentsIds = new List<Guid>();

        if (allRoles.Count < 1) { return parentsIds; }

        foreach (var roleId in roleIds)
        {
            var parentId = allRoles
               .Where(r => r.Id == roleId)
               .Select(r => r.ParentId)
               .First();

            if (parentId != null) 
            {
                parentsIds.Add((Guid)parentId);
            }
        }

        return parentsIds;
    }

    private static void GetChildRoleIdsInMemory(Guid parentId, List<Role> allRoles, List<Guid> childRoles)
    {
        var children = allRoles
            .Where(r => r.ParentId == parentId)
            .Select(r => r.Id)
            .ToList();

        foreach (var child in children)
        {
            if (!childRoles.Contains(child))
            {
                childRoles.Add(child);
                GetChildRoleIdsInMemory(child, allRoles, childRoles);
            }
        }
    }

}
