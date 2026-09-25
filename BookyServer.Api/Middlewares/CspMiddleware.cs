namespace BookyServer.Api.Middlewares;

/// <summary>
/// Middleware that adds response-security headers.
/// </summary>
public sealed class CspMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of <see cref="CspMiddleware"/>.
    /// </summary>
    /// <param name="next">The next request delegate.</param>
    /// <exception cref="ArgumentNullException">Thrown when the dependency is null.</exception>
    public CspMiddleware(RequestDelegate next)
    {
        this._next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Remove(Constants.Headers.PoweredBy);
        context.Response.Headers.Remove(Constants.Headers.Server);

        if (!context.Response.Headers.ContainsKey(Constants.Headers.ContentSecurityPolicy))
        {
            context.Response.Headers.Append(
                Constants.Headers.ContentSecurityPolicy,
                Constants.Headers.ContentSecurityPolicyValue);
        }

        if (!context.Response.Headers.ContainsKey(Constants.Headers.FrameOptions))
        {
            context.Response.Headers.Append(Constants.Headers.FrameOptions, Constants.Headers.FrameOptionsValue);
        }

        if (!context.Response.Headers.ContainsKey(Constants.Headers.XssProtection))
        {
            context.Response.Headers.Append(Constants.Headers.XssProtection, Constants.Headers.XssProtectionValue);
        }

        if (!context.Response.Headers.ContainsKey(Constants.Headers.ContentTypeOptions))
        {
            context.Response.Headers.Append(Constants.Headers.ContentTypeOptions, Constants.Headers.ContentTypeOptionsValue);
        }

        if (!context.Response.Headers.ContainsKey(Constants.Headers.StrictTransportSecurity))
        {
            context.Response.Headers.Append(
                Constants.Headers.StrictTransportSecurity,
                Constants.Headers.StrictTransportSecurityValue);
        }

        await this._next(context);
    }
}
