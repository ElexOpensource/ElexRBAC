using Newtonsoft.Json;
using RbacDashboard.Common;
using RbacDashboard.DAL.Commands;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Models.Domain;

namespace RbacDashboard.BAL;

public class AccessTokenRepository(IMediatorService mediator, IRbacTokenRepository tokenRepository) : IRbacAccessTokenRepository
{
    private readonly IMediatorService _mediator = mediator;

    private readonly IRbacTokenRepository _token = tokenRepository;

    public async Task<string> GetByRoleIds(List<Guid> roleIds)
    {
        var roleAccesses = await _mediator.SendRequest(new GetRoleAccessByRoleIds(roleIds));

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
        var accessMetaData = roleAccesses
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
                    CanInherit = item.AccessJson.CanInherit,
                    GenerateToken = item.AccessJson.GenerateToken
                })
                .GroupBy(item => item.AccessId)
                .Select(group => new AccessMetaData
                {
                    AccessId = group.Key,
                    AccessName = group.First().AccessName,
                    Permissions = group.SelectMany(item => item.Permissions).GroupBy(p => p.Id).Select(pGroup => pGroup.First()).ToList(),
                    CanInherit = group.First().CanInherit,
                    GenerateToken = group.First().GenerateToken
                })
                .ToList();
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (accessMetaData.Count == 0)
        {
            return string.Empty;
        }

        return _token.GenerateJwtToken(JsonConvert.SerializeObject(accessMetaData));
    }
}
