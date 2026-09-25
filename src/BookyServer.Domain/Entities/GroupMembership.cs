namespace BookyServer.Domain.Entities;

public sealed class GroupMembership
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    public ReaderGroup Group { get; set; } = null!;
}
