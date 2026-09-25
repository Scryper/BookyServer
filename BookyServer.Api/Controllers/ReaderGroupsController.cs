using BookyServer.Infrastructure.Identity;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Authorize]
[Route(Constants.Routes.ReaderGroups)]
public sealed class ReaderGroupsController(
    IReaderGroupService groups,
    UserManager<BookyUser> userManager) : ControllerBase
{
    private readonly IReaderGroupService _groups = groups ?? throw new ArgumentNullException(nameof(groups));
    private readonly UserManager<BookyUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ReaderGroupDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAsync(CancellationToken cancellationToken)
    {
        return Ok(await this._groups.GetActiveAsync(cancellationToken));
    }

    [HttpGet(Constants.Routes.ReaderGroupMembers)]
    [ProducesResponseType<IReadOnlyList<ReaderGroupMemberDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMembersAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var members = await this._groups.GetMembersAsync(groupId, cancellationToken);
        return members is null ? NotFound() : Ok(members);
    }

    [HttpPost(Constants.Routes.ReaderGroupJoin)]
    [ProducesResponseType<JoinGroupResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> JoinAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(this._userManager.GetUserId(this.User)!);
        var result = await this._groups.JoinAsync(groupId, userId, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return result.IsFull
            ? Conflict(new ProblemDetails { Title = Constants.Errors.ReaderGroupAtCapacity, Status = 409 })
            : Ok(result);
    }

    [HttpDelete(Constants.Routes.ReaderGroupJoin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LeaveAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(this._userManager.GetUserId(this.User)!);
        var left = await this._groups.LeaveAsync(groupId, userId, cancellationToken);
        return left ? NoContent() : NotFound();
    }
}
