namespace BookyServer.Interfaces.Contracts;

public sealed record ReaderGroupDto(
    Guid Id,
    string Name,
    string Topic,
    string? City,
    int MemberCount,
    int MaxMembers);

public sealed record ReaderGroupMemberDto(Guid UserId, string? DisplayName, DateTimeOffset JoinedAt);
public sealed record JoinGroupResult(bool Joined, bool IsFull);
