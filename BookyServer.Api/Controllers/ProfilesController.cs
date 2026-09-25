using System.Security.Claims;

using BookyServer.Infrastructure.Identity;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Route(Constants.Routes.Profiles)]
public sealed class ProfilesController(
    IProfileService profiles,
    UserManager<BookyUser> userManager) : ControllerBase
{
    private readonly IProfileService _profiles = profiles ?? throw new ArgumentNullException(nameof(profiles));
    private readonly UserManager<BookyUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    [HttpGet(Constants.Routes.CurrentProfile)]
    [Authorize]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMineAsync(CancellationToken cancellationToken)
    {
        var userId = this.GetCurrentUserId();
        var profile = await this._profiles.GetMineAsync(userId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut(Constants.Routes.CurrentProfile)]
    [Authorize]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMineAsync(
        UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await this._profiles.UpdateMineAsync(this.GetCurrentUserId(), request, cancellationToken);
            return Ok(profile);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = Constants.Errors.InvalidProfile,
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPut(Constants.Routes.ProfileBook)]
    [Authorize]
    [ProducesResponseType<ProfileBookDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RateBookAsync(
        Guid bookId, RateProfileBookRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await this._profiles.RateBookAsync(
                this.GetCurrentUserId(), bookId, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = Constants.Errors.InvalidBookRating,
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpGet(Constants.Routes.ProfileById)]
    [AllowAnonymous]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var viewerId = this.User.Identity?.IsAuthenticated == true ? this.TryGetCurrentUserId() : null;
        var profile = await this._profiles.GetByIdAsync(id, viewerId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    private Guid GetCurrentUserId()
    {
        return Guid.TryParse(this._userManager.GetUserId(this.User), out var userId)
            ? userId
            : throw new InvalidOperationException(Constants.Errors.InvalidCurrentUser);
    }

    private Guid? TryGetCurrentUserId()
    {
        return Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
    }
}
