using Radzen;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using RbacDashboard.DAL.Models.Domain;
using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Common.Authentication;

/// <summary>
/// Provides security services for RBAC, including user authentication and navigation management.
/// Initializes a new instance of the <see cref="RbacSecurityService"/> class.
/// </summary>
/// <param name="navigationManager">The navigation manager for handling navigation within the application.</param>
/// <param name="factory">The HTTP client factory for creating HTTP clients.</param>
public partial class RbacSecurityService(NavigationManager navigationManager, IHttpClientFactory factory)
{
    private readonly HttpClient httpClient = factory.CreateClient("Rbac");

    private readonly NavigationManager navigationManager = navigationManager;

    /// <summary>
    /// Gets the current user information.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Private member no need to included in the code coverage.")]
    public RbacApplicationUser User { get; private set; } = new RbacApplicationUser { CustomerId = string.Empty , CustomerName = "Anonymous" };

    /// <summary>
    /// Gets the current claims principal.
    /// </summary>
    public ClaimsPrincipal? Principal { get; private set; }

    /// <summary>
    /// Determines whether the current user is authenticated.
    /// </summary>
    /// <returns>True if the user is authenticated; otherwise, false.</returns>
    public bool IsAuthenticated() => Principal?.Identity?.IsAuthenticated == true;

    /// <summary>
    /// Initializes the authentication state.
    /// </summary>
    /// <param name="result">The authentication state.</param>
    /// <returns>True if the user is authenticated; otherwise, false.</returns>
    public bool InitializeAsync(AuthenticationState result)
    {
        Principal = result.User;

        if (IsAuthenticated())
        {
            User.CustomerId = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)!;
            User.CustomerName = Principal?.FindFirstValue(ClaimTypes.Name)!;
            User.ApplicationId = Principal?.FindFirstValue(RbacConstants.ApplicationId)!;
            User.ApplicationName = Principal?.FindFirstValue(RbacConstants.ApplicationName)!;
        }

        return IsAuthenticated();
    }

    /// <summary>
    /// Gets the customer ID of the current user.
    /// </summary>
    /// <returns>The customer ID.</returns>
    [ExcludeFromCodeCoverage(Justification = "Due to the limitations of the 'NavigationManager', it cannot be mocked effectively for unit testing.")]
    public string GetCustomerId() 
    {
        if (!IsAuthenticated() || User.CustomerId is null)
            Logout();
        
        return User.CustomerId!;
    }

    /// <summary>
    /// Gets the application ID of the current user.
    /// </summary>
    /// <returns>The application ID.</returns>
    [ExcludeFromCodeCoverage(Justification = "Due to the limitations of the 'NavigationManager', it cannot be mocked effectively for unit testing.")]
    public string GetApplicationId()
    {
        if (!IsAuthenticated())
            Logout();

        if (User.ApplicationId is null)
            navigationManager.NavigateTo("/", true);

        return User.ApplicationId!;
    }

    /// <summary>
    /// Gets the current authentication state asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ApplicationAuthenticationState"/>.</returns>
    [ExcludeFromCodeCoverage(Justification = "Due to the limitations of the 'NavigationManager', it cannot be mocked effectively for unit testing.")]
    public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
    {
        var uri = new Uri($"{navigationManager.BaseUri}Rbacapi/Account/CurrentUser");
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri));
        return await response.ReadAsync<ApplicationAuthenticationState>();
    }

    /// <summary>
    /// Logs the user out and navigates to the login page.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Due to the limitations of the 'NavigationManager', it cannot be mocked effectively for unit testing.")]
    public void Logout() => navigationManager.NavigateTo("RbacLogin", true);

    /// <summary>
    /// Changes the current application and navigates to the specified application ID.
    /// </summary>
    /// <param name="applicationId">The application ID to change to.</param>
    [ExcludeFromCodeCoverage(Justification = "Due to the limitations of the 'NavigationManager', it cannot be mocked effectively for unit testing.")]
    public void ChangeApplication(Guid? ApplicationId) => navigationManager.NavigateTo($"RbacLogin/ChangeApplication?ApplicationId={ApplicationId}", forceLoad: true);

    /// <summary>
    /// Changes the current application and navigates to the base URI.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Due to the limitations of the 'NavigationManager', it cannot be mocked effectively for unit testing.")]
    public void ChangeApplication() => navigationManager.NavigateTo(navigationManager.BaseUri);
}