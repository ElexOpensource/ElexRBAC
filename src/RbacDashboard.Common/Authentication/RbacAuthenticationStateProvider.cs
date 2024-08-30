using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using RbacDashboard.DAL.Models.Domain;

namespace RbacDashboard.Common.Authentication;

/// <summary>
/// Provides an authentication state provider for RBAC security service.
/// Initializes a new instance of the <see cref="RbacAuthenticationStateProvider"/> class.
/// </summary>
/// <param name="securityService">The security service to be used for authentication.</param>
[ExcludeFromCodeCoverage(Justification = "Due to limitations with RbacSecurityService, it cannot be mocked for test cases.")]
public class RbacAuthenticationStateProvider(RbacSecurityService securityService) : AuthenticationStateProvider
{
    private readonly RbacSecurityService securityService = securityService;
    private ApplicationAuthenticationState? authenticationState;

    /// <summary>
    /// Gets the current authentication state asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current <see cref="AuthenticationState"/>.</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();

        try
        {
            var state = await GetApplicationAuthenticationStateAsync();

            if (state.IsAuthenticated)
            {
                identity = new ClaimsIdentity(state.Claims.Select(c => new Claim(c.Type, c.Value)), "Rbac");
            }
        }
        catch (HttpRequestException)
        {
        }

        var result = new AuthenticationState(new ClaimsPrincipal(identity));

        securityService.InitializeAsync(result);

        return result;
    }

    /// <summary>
    /// Gets the application authentication state asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current <see cref="ApplicationAuthenticationState"/>.</returns>
    private async Task<ApplicationAuthenticationState> GetApplicationAuthenticationStateAsync()
    {
        authenticationState ??= await securityService.GetAuthenticationStateAsync();

        return authenticationState;
    }
}