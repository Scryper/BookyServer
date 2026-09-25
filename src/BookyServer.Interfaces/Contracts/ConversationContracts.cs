namespace BookyServer.Interfaces.Contracts;

public sealed record ConversationMessageDto(
    Guid Id,
    Guid GroupId,
    Guid AuthorUserId,
    string Text,
    DateTimeOffset CreatedAt);

public sealed record SendConversationMessageRequest(string Text);
