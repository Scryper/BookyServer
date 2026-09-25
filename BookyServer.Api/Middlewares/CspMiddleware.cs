namespace BookyServer.Api.Middlewares;

/// <summary>
/// Middleware that protects from XSS attacks.
/// </summary>
public class CspMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of <see cref="AntiXssMiddleware"/>.
    /// </summary>
    /// <param name="next">The next delegate.</param>
    /// <exception cref="ArgumentNullException">Exception thrown if DI breaks.</exception>
    public CspMiddleware(RequestDelegate next)
    {
        this._next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
		// Remove
		context.Response.Headers.Remove("X-Powered-By");
		context.Response.Headers.Remove("Server");

		// Add
		if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
		{
			context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
		}

		if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
		{
			context.Response.Headers.Append("X-Frame-Options", "DENY");
		}

		if (!context.Response.Headers.ContainsKey("X-XSS-Protection"))
		{
			context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
		}

		if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
		{
			context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
		}

		if (!context.Response.Headers.ContainsKey("Strict-Transport-Security"))
		{
			context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
		}

		await this._next(context);
    }
}
