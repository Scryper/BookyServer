namespace BookyServer.Domain.Entities;

public sealed class ReaderGroup
{
    public const int DefaultMaxMembers = 8;

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string? City { get; set; }
    public int MaxMembers { get; set; } = DefaultMaxMembers;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<GroupMembership> Members { get; set; } = new List<GroupMembership>();
    public ICollection<ConversationMessage> Messages { get; set; } = new List<ConversationMessage>();
}
