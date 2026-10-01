namespace BookyServer.Interfaces.Contracts;

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
