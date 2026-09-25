using BookyServer.Application.Mapping;
using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class ConversationService(IConversationRepository conversations) : IConversationService
{
    public async Task<IReadOnlyList<ConversationMessageDto>?> GetMessagesAsync(
        Guid groupId, Guid userId, int limit, CancellationToken cancellationToken)
    {
        if (!await conversations.IsMemberAsync(groupId, userId, cancellationToken))
        {
            return null;
        }

        var messages = await conversations.GetMessagesAsync(
            groupId, Math.Clamp(limit, 1, 100), cancellationToken);
        return messages.Select(ConversationMessageMapper.ToDto).ToArray();
    }

    public async Task<ConversationMessageDto?> SendAsync(
        Guid groupId, Guid userId, string text, CancellationToken cancellationToken)
    {
        if (!await conversations.IsMemberAsync(groupId, userId, cancellationToken))
        {
            return null;
        }

        var normalized = text.Trim();
        if (normalized.Length is 0 or > 3000)
        {
            throw new ArgumentException("Un message doit contenir de 1 à 3000 caractères.");
        }

        var message = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            AuthorUserId = userId,
            Text = normalized,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await conversations.AddMessageAsync(message, cancellationToken);
        return ConversationMessageMapper.ToDto(message);
    }
}
