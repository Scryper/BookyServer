using System.Security.Claims;

using BookyServer.Infrastructure.Identity;

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
    UserManager<BookyUser> userManager) : ControllerBase
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
    private readonly SignInManager<BookyUser> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    private readonly UserManager<BookyUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    [HttpGet("google")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GoogleAsync()
    {
        if (await this._authenticationSchemeProvider.GetSchemeAsync(GoogleDefaults.AuthenticationScheme) is null)
        {
            return this.Problem(
                title: Constants.Errors.GoogleAuthenticationUnavailable,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var redirectUrl = this.Url.RouteUrl(nameof(GoogleCallbackAsync))
            ?? throw new InvalidOperationException(Constants.Errors.GoogleAuthenticationFailed);
        var properties = this._signInManager.ConfigureExternalAuthenticationProperties(
            GoogleDefaults.AuthenticationScheme,
            redirectUrl);

        return this.Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google/callback", Name = nameof(GoogleCallbackAsync))]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleCallbackAsync(string? remoteError)
    {
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
            return this.NoContent();
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
        return this.NoContent();
    }

    private IActionResult GoogleAuthenticationFailed()
    {
        return this.Problem(
            title: Constants.Errors.GoogleAuthenticationFailed,
            statusCode: StatusCodes.Status400BadRequest);
    }
}
