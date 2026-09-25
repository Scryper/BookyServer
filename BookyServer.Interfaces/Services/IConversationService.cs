using BookyServer.Interfaces.Contracts;

namespace BookyServer.Interfaces.Services;

public interface IConversationService
{
    Task<IReadOnlyList<ConversationMessageDto>?> GetMessagesAsync(
        Guid groupId, Guid userId, int limit, CancellationToken cancellationToken);
    Task<ConversationMessageDto?> SendAsync(
        Guid groupId, Guid userId, string text, CancellationToken cancellationToken);
}
