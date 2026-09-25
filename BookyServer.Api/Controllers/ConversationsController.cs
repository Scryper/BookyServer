using BookyServer.Infrastructure.Identity;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/groups/{groupId:guid}/messages")]
public sealed class ConversationsController(
    IConversationService conversations,
    UserManager<BookyUser> userManager) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ConversationMessageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessages(
        Guid groupId, [FromQuery] int limit = 50, CancellationToken cancellationToken = default)
    {
        var result = await conversations.GetMessagesAsync(
            groupId, Guid.Parse(userManager.GetUserId(User)!), limit, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<ConversationMessageDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send(
        Guid groupId, SendConversationMessageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var message = await conversations.SendAsync(
                groupId, Guid.Parse(userManager.GetUserId(User)!), request.Text, cancellationToken);
            return message is null ? NotFound() : StatusCode(StatusCodes.Status201Created, message);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Message invalide",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
