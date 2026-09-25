namespace BookyServer.Interfaces.Contracts;

public sealed record UpdateProfileRequest(
    string FirstName,
    DateOnly BirthDate,
    string? Bio,
    string? City,
    bool IsPublic,
    bool HasVehicle,
    bool OffersCarpool,
    bool SeeksCarpool,
    IReadOnlyList<string> ReadingInterests,
    IReadOnlyList<string> OtherInterests);

public sealed record ProfileDto(
    Guid Id,
    string FirstName,
    int Age,
    string? Bio,
    string? City,
    bool IsPublic,
    bool HasVehicle,
    bool OffersCarpool,
    bool SeeksCarpool,
    IReadOnlyList<string> ReadingInterests,
    IReadOnlyList<string> OtherInterests,
    IReadOnlyList<ProfileBookDto> Books,
    IReadOnlyList<ProfilePhotoDto> Photos);

public sealed record ProfileBookDto(Guid BookId, string Title, string Author, string Rating, DateOnly? ReadAt);

public sealed record BookCatalogDto(
    Guid Id,
    string Title,
    string Author,
    int? PublicationYear,
    string? Genre,
    string? Synopsis,
    string? Isbn13,
    string? CoverUrl);

public sealed record RateProfileBookRequest(string Rating, DateOnly? ReadAt);

public sealed record ProfilePhotoDto(Guid Id, string Category, int SortOrder);
