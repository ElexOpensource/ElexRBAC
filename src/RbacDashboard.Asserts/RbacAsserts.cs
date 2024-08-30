using Microsoft.AspNetCore.Http;

namespace RbacDashboard.Asserts;

/// <summary>
/// The <see cref="RbacAsserts"/> class retrieves the assembly name for the RbacDashboard Assets. Do not modify or use it for other purposes.
/// </summary>
public class RbacAsserts { }

/// <summary>
/// Middleware to serve embedded static files from a Razor Class Library (RCL).
/// </summary>
public class RbacEmbeddedFileMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Processes HTTP requests and serves embedded static files if they match the
    /// requested path. If a file is found, it is served with the appropriate content type.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var requestPath = context.Request.Path.Value;

        var assembly = typeof(RbacAsserts).Assembly;
        var assemblyName = assembly.GetName().Name;

        if (requestPath!.StartsWith($"/_content/{assemblyName}"))
        {
            var resourcePath = requestPath.Replace($"/_content/{assemblyName}/", $"{assemblyName}.wwwroot.")
                                          .Replace("/", ".");

            var stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream != null)
            {
                var fileExtension = Path.GetExtension(requestPath);
                var contentType = fileExtension switch
                {
                    ".css" => "text/css",
                    ".js" => "application/javascript",
                    ".html" => "text/html",
                    _ => "application/octet-stream"
                };

                context.Response.ContentType = contentType;
                await stream.CopyToAsync(context.Response.Body);
                return;
            }
        }

        await _next(context);
    }
}