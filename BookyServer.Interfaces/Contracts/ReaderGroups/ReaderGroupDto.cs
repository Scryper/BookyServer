namespace BookyServer.Interfaces.Contracts;

public sealed record ReaderGroupDto(
    Guid Id,
    string Name,
    string Topic,
    string? City,
    int MemberCount,
    int MaxMembers);
