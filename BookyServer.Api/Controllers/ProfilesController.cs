using System.Security.Claims;
using BookyServer.Infrastructure.Identity;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Route("api/v1/profiles")]
public sealed class ProfilesController(
    IProfileService profiles,
    UserManager<BookyUser> userManager) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await profiles.GetMineAsync(userId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMine(
        UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await profiles.UpdateMineAsync(GetCurrentUserId(), request, cancellationToken);
            return Ok(profile);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Profil invalide",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPut("me/books/{bookId:guid}")]
    [Authorize]
    [ProducesResponseType<ProfileBookDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RateBook(
        Guid bookId, RateProfileBookRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await profiles.RateBookAsync(
                GetCurrentUserId(), bookId, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Avis de lecture invalide",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType<ProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var viewerId = User.Identity?.IsAuthenticated == true ? TryGetCurrentUserId() : null;
        var profile = await profiles.GetByIdAsync(id, viewerId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    private Guid GetCurrentUserId() =>
        Guid.TryParse(userManager.GetUserId(User), out var userId)
            ? userId
            : throw new InvalidOperationException("L'identifiant utilisateur de la session est invalide.");

    private Guid? TryGetCurrentUserId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
}
