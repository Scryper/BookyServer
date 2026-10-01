namespace BookyServer.Api.Middlewares;

/// <summary>
/// Middleware that adds response-security headers.
/// </summary>
public sealed class CspMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("Server");

        if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            context.Response.Headers.Append(
                "Content-Security-Policy",
                "default-src 'self'");
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
            context.Response.Headers.Append(
                "Strict-Transport-Security",
                "max-age=31536000; includeSubDomains");
        }

        await this._next(context);
    }
}
