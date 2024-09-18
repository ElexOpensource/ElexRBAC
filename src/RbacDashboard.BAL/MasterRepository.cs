using RbacDashboard.Common;
using RbacDashboard.DAL.Models;
using RbacDashboard.DAL.Commands;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Models.Domain;

namespace RbacDashboard.BAL;

public class MasterRepository(IMediatorService mediator, IRbacTokenRepository tokenRepository) : IRbacMasterRepository
{
    private readonly IMediatorService _mediator = mediator;
    private readonly IRbacTokenRepository _tokenRepository = tokenRepository;

    public async Task<string> GenetrateTokenByCustomer(Guid customerId)
    {
        var customer = await _mediator.SendRequest(new GetCustomerById(customerId));
        return customer is null
            ? throw new KeyNotFoundException($"Customer with ID {customerId} was not found.")
            : _tokenRepository.GenerateJwtToken(customerId.ToString(), RbacConstants.CustomerId, 120);
    }

    public async Task<List<Option>> GetOptions(Guid applicationId, string optionName)
    {
        return optionName.ToUpper() switch
        {
            "ROLE" => (await _mediator.SendRequest(new GetRolesByApplicationId(applicationId)))
                        .Select(role => new Option { Id = role.Id, Label = role.Name })
                        .ToList(),
            "ACCESS" => (await _mediator.SendRequest(new GetAccessesByApplicationId(applicationId)))
                        .Select(access => new Option { Id = access.Id, Label = access.Name })
                        .ToList(),
            _ => throw new Exception($"Invalid Option Name - {optionName} : it must be `ROLE` or `ACCESS`"),
        };
    }

    public async Task<List<OptionsetMaster>> GetOptionsetMasters()
    {
        var optionsets = await _mediator.SendRequest(new GetOptionsetMasters());
        return optionsets;
    }

    public async Task<List<Permissionset>> GetPermissionSetList()
    {
        var permissions = await _mediator.SendRequest(new GetPermissionsets());
        return permissions;
    }

    public async Task<List<TypeMaster>> GetTypeMasters()
    {
        var types = await _mediator.SendRequest(new GetAllTypeMaster());
        return types;
    }
}