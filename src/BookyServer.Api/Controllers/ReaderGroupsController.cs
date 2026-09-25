using BookyServer.Infrastructure.Identity;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/groups")]
public sealed class ReaderGroupsController(
    IReaderGroupService groups,
    UserManager<BookyUser> userManager) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ReaderGroupDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken) =>
        Ok(await groups.GetActiveAsync(cancellationToken));

    [HttpGet("{groupId:guid}/members")]
    [ProducesResponseType<IReadOnlyList<ReaderGroupMemberDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMembers(Guid groupId, CancellationToken cancellationToken)
    {
        var members = await groups.GetMembersAsync(groupId, cancellationToken);
        return members is null ? NotFound() : Ok(members);
    }

    [HttpPost("{groupId:guid}/join")]
    [ProducesResponseType<JoinGroupResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Join(Guid groupId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(userManager.GetUserId(User)!);
        var result = await groups.JoinAsync(groupId, userId, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return result.IsFull
            ? Conflict(new ProblemDetails { Title = "Le groupe a atteint sa capacité maximale.", Status = 409 })
            : Ok(result);
    }

    [HttpDelete("{groupId:guid}/join")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Leave(Guid groupId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(userManager.GetUserId(User)!);
        var left = await groups.LeaveAsync(groupId, userId, cancellationToken);
        return left ? NoContent() : NotFound();
    }
}
