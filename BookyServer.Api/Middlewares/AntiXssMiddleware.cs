using System.Net;
using System.Text;
using BookyServer.Api.Helpers;
using Institut.Helpers;
using Institut.Helpers.Models;

namespace BookyServer.Api.Middlewares;

/// <summary>
/// Middleware that protects from XSS attacks.
/// </summary>
public class AntiXssMiddleware
{
    private readonly RequestDelegate _next;
    private ErrorResponse? _error;
    private const int StatusCode = (int)HttpStatusCode.BadRequest;

    /// <summary>
    /// Initializes a new instance of <see cref="AntiXssMiddleware"/>.
    /// </summary>
    /// <param name="next">The next delegate.</param>
    /// <exception cref="ArgumentNullException">Exception thrown if DI breaks.</exception>
    public AntiXssMiddleware(RequestDelegate next)
    {
        this._next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        // Check XSS in URL
        if (!string.IsNullOrWhiteSpace(context.Request.Path.Value))
        {
            var url = context.Request.Path.Value;

            if (CrossSiteScriptingValidation.IsDangerousString(url, out _))
            {
                await this.RespondWithAnError(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in query string
        if (!string.IsNullOrWhiteSpace(context.Request.QueryString.Value))
        {
            var queryString = WebUtility.UrlDecode(context.Request.QueryString.Value);

            if (CrossSiteScriptingValidation.IsDangerousString(queryString, out _))
            {
                await this.RespondWithAnError(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in request content
        var originalBody = context.Request.Body;
        try
        {
            var content = await ReadRequestBody(context);

            if (!content.Contains("Content-Disposition: form-data") && CrossSiteScriptingValidation.IsDangerousString(content, out _)) 
            {
                await this.RespondWithAnError(context);
                return;
            }
            await this._next(context);
        }
        finally
        {
            context.Request.Body = originalBody;
        }
    }

    private static async Task<string> ReadRequestBody(HttpContext context)
    {
        var buffer = new MemoryStream();
        await context.Request.Body.CopyToAsync(buffer);
        context.Request.Body = buffer;
        buffer.Position = 0;

        var encoding = Encoding.UTF8;

        var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
        context.Request.Body.Position = 0;

        return requestContent;
    }

    private async Task RespondWithAnError(HttpContext context)
    {
        context.Response.Clear();
        context.Response.Headers.AddHeaders();
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = StatusCode;

        this._error ??= new ErrorResponse
        {
            Description = "Error from AntiXssMiddleware",
            ErrorCode = 500
        };

        await context.Response.WriteAsync(this._error.ToJson());
    }
}