using BookyServer.Application.Mapping;
using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class ConversationService(IConversationRepository conversationRepository) : IConversationService
{
    private readonly IConversationRepository _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));

    public async Task<IReadOnlyList<ConversationMessageDto>?> GetMessagesAsync(Guid groupId, Guid userId, int limit, CancellationToken cancellationToken)
    {
        if (!await this._conversationRepository.IsMemberAsync(groupId, userId, cancellationToken))
        {
            return null;
        }

        var messages = await this._conversationRepository.GetMessagesAsync(groupId, Math.Clamp(limit, 1, 100), cancellationToken);
        return messages.Select(ConversationMessageMapper.Map).ToArray();
    }

    public async Task<ConversationMessageDto?> SendAsync(Guid groupId, Guid userId, string text, CancellationToken cancellationToken)
    {
        if (!await this._conversationRepository.IsMemberAsync(groupId, userId, cancellationToken))
        {
            return null;
        }

        var normalized = text.Trim();
        if (normalized.Length is 0 or > 3000)
        {
            throw new ArgumentException(Constants.Errors.InvalidConversationMessage);
        }

        var message = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            AuthorUserId = userId,
            Text = normalized,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await this._conversationRepository.AddMessageAsync(message, cancellationToken);
        return ConversationMessageMapper.Map(message);
    }
}
