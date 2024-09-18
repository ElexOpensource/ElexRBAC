using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RbacDashboard.DAL.Commands;
using RbacDashboard.Common.Interface;
using RbacDashboard.DAL.Models.Domain;

namespace RbacDashboard.Common.Authentication;

public class RbacSignInManager( UserManager<RbacApplicationUser> userManager, 
    IHttpContextAccessor contextAccessor,  IUserClaimsPrincipalFactory<RbacApplicationUser> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<RbacApplicationUser>> logger, 
    IAuthenticationSchemeProvider schemes, IUserConfirmation<RbacApplicationUser> confirmation,
    IMediatorService mediator, IRbacTokenRepository tokenRepository
) : SignInManager<RbacApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
{
    private readonly UserManager<RbacApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly IUserClaimsPrincipalFactory<RbacApplicationUser> _claimsFactory = claimsFactory;
    private readonly IOptions<IdentityOptions> _optionsAccessor = optionsAccessor;
    private readonly ILogger<SignInManager<RbacApplicationUser>> _logger = logger;
    private readonly IAuthenticationSchemeProvider _schemes = schemes;
    private readonly IUserConfirmation<RbacApplicationUser> _confirmation = confirmation;
    private readonly IRbacTokenRepository _tokenRepository = tokenRepository;
    private readonly IMediatorService _mediator = mediator;

    public override async Task<SignInResult> PasswordSignInAsync(string userId, string appId, bool isPersistent, bool lockoutOnFailure)
    {
        try
        {
            var principal = _tokenRepository.ValidateJwtToken(userId);

            if (principal.Identity?.IsAuthenticated == false)
            {
                Console.WriteLine($"\n Invalid Token");
                return SignInResult.Failed;
            }

            var customerId = principal.FindFirst(RbacConstants.CustomerId)?.Value;
            Console.WriteLine($"\n Customer Id: {customerId}");
            if (string.IsNullOrEmpty(customerId))
            {
                Console.WriteLine($"\n Customer not found in the Token");
                return SignInResult.Failed;
            }

            var customer = await _mediator.SendRequest(new GetCustomerById(Guid.Parse(customerId)));

            if (customer == null)
            {
                Console.WriteLine($"\n Customer not found {customerId}");
                return SignInResult.Failed;
            }

            var user = new RbacApplicationUser
            {
                CustomerId = customer.Id.ToString(),
                CustomerName = customer.Name,
                ApplicationId = string.Empty,
                ApplicationName = string.Empty,
            };

            await base.SignInAsync(user, false);
            return SignInResult.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n Authentication Exception : {ex.ToString()}");
            Console.WriteLine($"\n Authentication Error Message : {ex.Message}");
            Console.WriteLine($"\n Authentication Trace : {ex.StackTrace}");
            return SignInResult.Failed;
        }
    }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override async Task<SignInResult> SignInWithClaimsAsync(RbacApplicationUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    {
        var principal = await _claimsFactory.CreateAsync(user);
        if (user.ApplicationId is not null)
            ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(RbacConstants.ApplicationId, user.ApplicationId));

        if (user.ApplicationName is not null)
            ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(RbacConstants.ApplicationName, user.ApplicationName));

        authenticationProperties ??= new AuthenticationProperties();
        authenticationProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(RbacConstants.CookieLifeSpanInMinutes);
        await _contextAccessor.HttpContext!.SignInAsync(RbacConstants.AuthenticationSchema, principal, authenticationProperties);

        return SignInResult.Success;
    }

    public override async Task RefreshSignInAsync(RbacApplicationUser user)
    {
        var customer = await _mediator.SendRequest(new GetCustomerById(Guid.Parse(user.CustomerId)));

        if (customer is null || user.ApplicationId is null)
        {
            return;
        }

        var app = await _mediator.SendRequest(new GetApplicationById(Guid.Parse(user.ApplicationId)));

        user.ApplicationId = app?.Id.ToString()!;
        user.ApplicationName = app?.Name?.ToString()!;

        var principal = await _claimsFactory.CreateAsync(user);
        ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(RbacConstants.ApplicationId, user.ApplicationId));
        ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(RbacConstants.ApplicationName, user.ApplicationName));

        await _contextAccessor.HttpContext!.SignInAsync(RbacConstants.AuthenticationSchema, principal);

        await base.RefreshSignInAsync(user);         
    }

    public override async Task SignOutAsync()
    {
        await _contextAccessor.HttpContext!.SignOutAsync(RbacConstants.AuthenticationSchema);
    }
}
