using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class ConversationMessageMapper
{
    public static ConversationMessageDto ToDto(ConversationMessage message) => new(
        message.Id,
        message.GroupId,
        message.AuthorUserId,
        message.Text,
        message.CreatedAt);
}
