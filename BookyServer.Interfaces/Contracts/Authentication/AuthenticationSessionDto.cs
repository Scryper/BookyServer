namespace BookyServer.Interfaces.Contracts.Authentication;

public sealed record AuthenticationSessionDto(bool IsAuthenticated, Guid? UserId);

