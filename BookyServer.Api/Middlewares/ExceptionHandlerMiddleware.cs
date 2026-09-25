using System.Net;
using System.Text.Json;

namespace BookyServer.Api.Middlewares;

/// <summary>
/// Middleware that catches all exception from application that are not catch for custom handling.
/// </summary>
public class ExceptionHandlerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlerMiddleware> _logger;

	/// <summary>
	/// Initializes a new instance of <see cref="ExceptionHandlerMiddleware"/>.
	/// </summary>
	/// <param name="next">The request delegate used in middlewares pipe.</param>
	/// <param name="logger">The logger.</param>
	/// <exception cref="ArgumentNullException">Exception thrown if DI breaks.</exception>
	public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
	{
		this._next = next ?? throw new ArgumentNullException(nameof(next));
		this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <summary>
	/// Handler.
	/// </summary>
	/// <param name="context">Current HTTP context.</param>
	public async Task Invoke(HttpContext context)
	{
		try
		{
			await this._next(context);
		}
		catch (Exception error)
		{
			var response = context.Response;
			response.ContentType = "application/json";

			response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var result = JsonSerializer.Serialize(new { message = error.Message });
			this._logger.LogError("{Message}\n{StackTrace}", error.Message, error.StackTrace);
			await response.WriteAsync(result);
		}
	}
}