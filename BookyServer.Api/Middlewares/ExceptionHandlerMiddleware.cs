using System.Net;
using System.Text.Json;

namespace BookyServer.Api.Middlewares;

/// <summary>
/// Middleware that handles unhandled application exceptions.
/// </summary>
public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ExceptionHandlerMiddleware"/>.
    /// </summary>
    /// <param name="next">The next request delegate.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        this._next = next ?? throw new ArgumentNullException(nameof(next));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the request and writes an error response for unhandled exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this._next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = Constants.ResponseContentTypes.Json;
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new { message = error.Message });
            this._logger.LogError(Constants.Logging.UnhandledException, error.Message, error.StackTrace);
            await response.WriteAsync(result);
        }
    }
}
