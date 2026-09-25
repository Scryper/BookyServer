namespace BookyServer.Interfaces.Contracts;

public sealed record ProfileBookDto(Guid BookId, string Title, string Author, string Rating, DateOnly? ReadAt);
