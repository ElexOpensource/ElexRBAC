using RbacDashboard.Common;
using Microsoft.AspNetCore.Mvc;
using RbacDashboard.DAL.Models.Domain;
using Microsoft.AspNetCore.Authentication;

namespace Rbac.Controllers;

[Route("/Rbacapi/[controller]/[action]")]
[ApiExplorerSettings(IgnoreApi = true)]
public partial class AccountController : Controller
{
    [HttpPost]
    public async Task<ApplicationAuthenticationState?> CurrentUser()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(RbacConstants.AuthenticationSchema);
        if (authenticateResult == null || !authenticateResult.Succeeded || authenticateResult.Principal == null)
        {
            return new ApplicationAuthenticationState
            {
                IsAuthenticated = false,
                Name = string.Empty,
                Claims = Enumerable.Empty<ApplicationClaim>()
            };
        }

        return new ApplicationAuthenticationState
        {
            IsAuthenticated = authenticateResult.Principal.Identity?.IsAuthenticated ?? false,
            Name = authenticateResult.Principal.Identity?.Name ?? string.Empty,
            Claims = authenticateResult.Principal.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value }) ?? Enumerable.Empty<ApplicationClaim>()
        };
    }
}
