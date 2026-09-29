using System.Security.Claims;

using BookyServer.Infrastructure.Identity;
using BookyServer.Interfaces.Contracts.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthenticationController(
    IAuthenticationSchemeProvider authenticationSchemeProvider,
    SignInManager<BookyUser> signInManager,
    UserManager<BookyUser> userManager,
    IConfiguration configuration) : ControllerBase
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
    private readonly SignInManager<BookyUser> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    private readonly UserManager<BookyUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    [HttpGet("google")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GoogleAsync(
        [FromQuery(Name = Constants.Authentication.ReturnUrlQueryParameter)] string? returnUrl)
    {
        return await this.GoogleChallengeAsync(nameof(GoogleCallbackAsync), returnUrl);
    }

    [HttpGet("google/callback", Name = nameof(GoogleCallbackAsync))]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleCallbackAsync(
        string? remoteError,
        [FromQuery(Name = Constants.Authentication.ReturnUrlQueryParameter)] string? returnUrl)
    {
        if (!this.IsAllowedFrontendRedirectUrl(returnUrl))
        {
            return this.InvalidAuthenticationRedirectUrl();
        }

        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            return this.GoogleAuthenticationFailed();
        }

        var login = await this._signInManager.GetExternalLoginInfoAsync();
        if (login is null)
        {
            return this.GoogleAuthenticationFailed();
        }

        var signInResult = await this._signInManager.ExternalLoginSignInAsync(
            login.LoginProvider,
            login.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);
        if (signInResult.Succeeded)
        {
            return this.GoogleAuthenticationSucceeded(returnUrl);
        }

        var email = login.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            return this.Problem(
                title: Constants.Errors.GoogleEmailMissing,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var user = new BookyUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };
        var createResult = await this._userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            return this.GoogleAuthenticationFailed();
        }

        var addLoginResult = await this._userManager.AddLoginAsync(user, login);
        if (!addLoginResult.Succeeded)
        {
            await this._userManager.DeleteAsync(user);
            return this.GoogleAuthenticationFailed();
        }

        await this._signInManager.SignInAsync(user, isPersistent: false);
        return this.GoogleAuthenticationSucceeded(returnUrl);
    }

    [HttpGet("google/login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GoogleLoginAsync(
        [FromQuery(Name = Constants.Authentication.ReturnUrlQueryParameter)] string? returnUrl)
    {
        return await this.GoogleChallengeAsync(nameof(GoogleLoginCallbackAsync), returnUrl);
    }

    [HttpGet("google/login/callback", Name = nameof(GoogleLoginCallbackAsync))]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleLoginCallbackAsync(
        string? remoteError,
        [FromQuery(Name = Constants.Authentication.ReturnUrlQueryParameter)] string? returnUrl)
    {
        if (!this.IsAllowedFrontendRedirectUrl(returnUrl))
        {
            return this.InvalidAuthenticationRedirectUrl();
        }

        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            return this.GoogleAuthenticationFailed();
        }

        var login = await this._signInManager.GetExternalLoginInfoAsync();
        if (login is null)
        {
            return this.GoogleAuthenticationFailed();
        }

        var signInResult = await this._signInManager.ExternalLoginSignInAsync(
            login.LoginProvider,
            login.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);
        if (!signInResult.Succeeded)
        {
            return this.Problem(
                title: Constants.Errors.GoogleAuthenticationFailed,
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return this.GoogleAuthenticationSucceeded(returnUrl);
    }

    [HttpGet("session")]
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ProducesResponseType<AuthenticationSessionDto>(StatusCodes.Status200OK)]
    public IActionResult Session()
    {
        var isAuthenticated = this.User.Identity?.IsAuthenticated == true;
        Guid? userId = null;
        if (isAuthenticated && Guid.TryParse(this._userManager.GetUserId(this.User), out var parsedUserId))
        {
            userId = parsedUserId;
        }

        return this.Ok(new AuthenticationSessionDto(isAuthenticated, userId));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAsync()
    {
        await this._signInManager.SignOutAsync();
        return this.NoContent();
    }

    private async Task<IActionResult> GoogleChallengeAsync(string callbackRouteName, string? returnUrl)
    {
        if (!this.IsAllowedFrontendRedirectUrl(returnUrl))
        {
            return this.InvalidAuthenticationRedirectUrl();
        }

        if (await this._authenticationSchemeProvider.GetSchemeAsync(GoogleDefaults.AuthenticationScheme) is null)
        {
            return this.Problem(
                title: Constants.Errors.GoogleAuthenticationUnavailable,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var redirectUrl = string.IsNullOrWhiteSpace(returnUrl)
            ? this.Url.RouteUrl(callbackRouteName)
            : this.Url.RouteUrl(callbackRouteName, new { returnUrl });
        if (redirectUrl is null)
        {
            throw new InvalidOperationException(Constants.Errors.GoogleAuthenticationFailed);
        }

        var properties = this._signInManager.ConfigureExternalAuthenticationProperties(
            GoogleDefaults.AuthenticationScheme,
            redirectUrl);

        return this.Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    private bool IsAllowedFrontendRedirectUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return true;
        }

        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var redirectUri)
            || (redirectUri.Scheme != Uri.UriSchemeHttp && redirectUri.Scheme != Uri.UriSchemeHttps)
            || !string.IsNullOrEmpty(redirectUri.UserInfo))
        {
            return false;
        }

        var allowedOrigins = this._configuration
            .GetSection(Constants.Configuration.AllowedCorsOrigins)
            .Get<string[]>() ?? [];

        return allowedOrigins.Any(allowedOrigin =>
            Uri.TryCreate(allowedOrigin, UriKind.Absolute, out var allowedOriginUri)
            && string.Equals(
                redirectUri.GetLeftPart(UriPartial.Authority),
                allowedOriginUri.GetLeftPart(UriPartial.Authority),
                StringComparison.OrdinalIgnoreCase));
    }

    private IActionResult GoogleAuthenticationSucceeded(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return this.NoContent();
        }

        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var redirectUri))
        {
            return this.Redirect(redirectUri.AbsoluteUri);
        }

        return this.InvalidAuthenticationRedirectUrl();
    }

    private IActionResult InvalidAuthenticationRedirectUrl()
    {
        return this.Problem(
            title: Constants.Errors.InvalidAuthenticationRedirectUrl,
            statusCode: StatusCodes.Status400BadRequest);
    }

    private IActionResult GoogleAuthenticationFailed()
    {
        return this.Problem(
            title: Constants.Errors.GoogleAuthenticationFailed,
            statusCode: StatusCodes.Status400BadRequest);
    }
}
