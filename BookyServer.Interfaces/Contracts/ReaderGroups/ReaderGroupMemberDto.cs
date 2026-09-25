namespace BookyServer.Interfaces.Contracts;

public sealed record ReaderGroupMemberDto(Guid UserId, string? DisplayName, DateTimeOffset JoinedAt);
