namespace BookyServer.Interfaces.Contracts;

public sealed record MapMarkerDto(
    Guid Id,
    string Name,
    string Category,
    string? Address,
    double Latitude,
    double Longitude,
    bool IsPartner,
    string? SourceUrl);

public sealed record CreateMapMarkerRequest(
    string Name,
    string Category,
    string? Address,
    double Latitude,
    double Longitude,
    bool IsPartner,
    string? SourceUrl);
