using BookyServer.Domain.Entities;

namespace BookyServer.Interfaces.Repositories;

public interface IConversationRepository
{
    Task<bool> IsMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ConversationMessage>> GetMessagesAsync(Guid groupId, int limit, CancellationToken cancellationToken);
    Task AddMessageAsync(ConversationMessage message, CancellationToken cancellationToken);
}
