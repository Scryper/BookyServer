using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class ConversationMessageMapper
{
    public static ConversationMessageDto Map(ConversationMessage message)
    {
        return new ConversationMessageDto(
            message.Id,
            message.GroupId,
            message.AuthorUserId,
            message.Text,
            message.CreatedAt);
    }
}
