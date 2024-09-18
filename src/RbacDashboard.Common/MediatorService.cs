using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace RbacDashboard.Common;

/// <summary>
/// Defines a mediator service interface for sending requests.
/// </summary>
public interface IMediatorService
{
    /// <summary>
    /// Sends a request through the mediator and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response.</returns>
    Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request);
}

/// <summary>
/// Implements the <see cref="IMediatorService"/> interface to send requests using MediatR.
/// Initializes a new instance of the <see cref="MediatorService"/> class.
/// </summary>
/// <param name="mediator">The mediator instance.</param>
/// <param name="logger">The logger instance.</param>
public class MediatorService(IMediator mediator, ILogger<MediatorService> logger) : IMediatorService
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<MediatorService> _logger = logger;

    /// <summary>
    /// Sends a request through the mediator and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the MediatR handler is not registered.</exception>
    /// <exception cref="ApplicationException">Thrown when an unexpected error occurs.</exception>
    public async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request)
    {
        try
        {
            return await _mediator.Send(request);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("No service for type 'MediatR.IRequestHandler`"))
        {
            string commandName = GetCommandNameFromExceptionMessage(ex.Message);
            _logger.LogError($"MediatR handler not registered: {commandName}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            throw;
        }
    }

    /// <summary>
    /// Extracts the command name from the exception message.
    /// </summary>
    /// <param name="errorMessage">The exception message.</param>
    /// <returns>The extracted command name, or "UnknownCommand" if extraction fails.</returns>
    private static string GetCommandNameFromExceptionMessage(string errorMessage)
    {
        string pattern = @"No service for type 'MediatR\.IRequestHandler`2\[(?<CommandName>[^\]]+)";
        Match match = Regex.Match(errorMessage, pattern);

        if (match.Success)
        {
            return match.Groups["CommandName"].Value;
        }

        return "UnknownCommand";
    }
}