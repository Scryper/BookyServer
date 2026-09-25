using System.Net;
using System.Text;
using BookyServer.Api.Helpers;
using BookyServer.Api.Helpers.Models;

namespace BookyServer.Api.Middlewares;

/// <summary>
/// Middleware that protects from XSS attacks.
/// </summary>
public sealed class AntiXssMiddleware
{
    private readonly RequestDelegate _next;
    private const int BadRequestStatusCode = (int)HttpStatusCode.BadRequest;

    /// <summary>
    /// Initializes a new instance of <see cref="AntiXssMiddleware"/>.
    /// </summary>
    /// <param name="next">The next delegate.</param>
    /// <exception cref="ArgumentNullException">Exception thrown if DI breaks.</exception>
    public AntiXssMiddleware(RequestDelegate next)
    {
        this._next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check XSS in URL
        if (!string.IsNullOrWhiteSpace(context.Request.Path.Value))
        {
            var url = context.Request.Path.Value;

            if (CrossSiteScriptingValidation.IsDangerousString(url, out _))
            {
                await this.RespondWithAnErrorAsync(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in query string
        if (!string.IsNullOrWhiteSpace(context.Request.QueryString.Value))
        {
            var queryString = WebUtility.UrlDecode(context.Request.QueryString.Value);

            if (CrossSiteScriptingValidation.IsDangerousString(queryString, out _))
            {
                await this.RespondWithAnErrorAsync(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in request content
        var originalBody = context.Request.Body;
        try
        {
            var content = await ReadRequestBodyAsync(context);

            if (!content.Contains(Constants.Requests.FormDataContentDisposition, StringComparison.Ordinal) &&
                CrossSiteScriptingValidation.IsDangerousString(content, out _))
            {
                await this.RespondWithAnErrorAsync(context);
                return;
            }

            await this._next(context);
        }
        finally
        {
            context.Request.Body = originalBody;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpContext context)
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

    private async Task RespondWithAnErrorAsync(HttpContext context)
    {
        context.Response.Clear();
        context.Response.Headers.AddHeaders();
        context.Response.ContentType = Constants.ResponseContentTypes.Utf8Json;
        context.Response.StatusCode = BadRequestStatusCode;

        var error = new ErrorResponse
        {
            Description = Constants.Errors.AntiXss,
            ErrorCode = 500
        };

        await context.Response.WriteAsync(error.ToJson());
    }
}
