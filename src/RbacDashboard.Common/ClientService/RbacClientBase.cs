using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Common.ClientService;

/// <summary>
/// Initializes a new instance of the <see cref="RbacClientBase"/> class.
/// </summary>
/// <param name="factory">The HTTP client factory used to create the client.</param>
[ExcludeFromCodeCoverage]
public class RbacClientBase(IHttpClientFactory factory)
{
    private readonly HttpClient _httpClient = factory.CreateClient("Rbac");

    /// <summary>
    /// Sends a GET request to the specified endpoint and deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the response content.</typeparam>
    /// <param name="endpoint">The endpoint to send the request to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response content.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the response content is null.</exception>

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response content was null.");
    }

    /// <summary>
    /// Sends a POST request with the specified data to the specified endpoint and deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the response content.</typeparam>
    /// <param name="endpoint">The endpoint to send the request to.</param>
    /// <param name="data">The data to include in the request body.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response content.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the response content is null.</exception>
    protected async Task<T> PostAsync<T>(string endpoint, object data)
    {
        var response = await _httpClient.PostAsync(endpoint, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response content was null.");
    }

    /// <summary>
    /// Sends a POST request with the specified data to the specified endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint to send the request to.</param>
    /// <param name="data">The data to include in the request body.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task PostAsync(string endpoint, object data)
    {
        var response = await _httpClient.PostAsync(endpoint, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Sends a DELETE request to the specified endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint to send the request to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
    }
}
