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
