namespace BookyServer.Interfaces.Contracts;

public sealed record BookCatalogDto(
    Guid Id,
    string Title,
    string Author,
    int? PublicationYear,
    string? Genre,
    string? Synopsis,
    string? Isbn13,
    string? CoverUrl);
