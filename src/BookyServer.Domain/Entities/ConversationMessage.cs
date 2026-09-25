namespace BookyServer.Domain.Entities;

public sealed class ConversationMessage
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ReaderGroup Group { get; set; } = null!;
}
