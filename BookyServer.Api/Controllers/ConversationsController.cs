using BookyServer.Infrastructure.Identity;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookyServer.Api.Controllers;

[ApiController]
[Authorize]
[Route(Constants.Routes.Conversations)]
public sealed class ConversationsController(
    IConversationService conversations,
    UserManager<BookyUser> userManager) : ControllerBase
{
    private readonly IConversationService _conversations =
        conversations ?? throw new ArgumentNullException(nameof(conversations));
    private readonly UserManager<BookyUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ConversationMessageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessagesAsync(
        Guid groupId, [FromQuery] int limit = 50, CancellationToken cancellationToken = default)
    {
        var result = await this._conversations.GetMessagesAsync(
            groupId, Guid.Parse(this._userManager.GetUserId(this.User)!), limit, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<ConversationMessageDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendAsync(
        Guid groupId, SendConversationMessageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var message = await this._conversations.SendAsync(
                groupId, Guid.Parse(this._userManager.GetUserId(this.User)!), request.Text, cancellationToken);
            return message is null ? NotFound() : StatusCode(StatusCodes.Status201Created, message);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = Constants.Errors.InvalidConversationMessage,
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
