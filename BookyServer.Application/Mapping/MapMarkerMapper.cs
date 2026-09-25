using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class MapMarkerMapper
{
    public static MapMarkerDto ToDto(MapMarker marker) => new(
        marker.Id,
        marker.Name,
        marker.Category,
        marker.Address,
        marker.Latitude,
        marker.Longitude,
        marker.IsPartner,
        marker.SourceUrl);

    public static MapMarker ToEntity(CreateMapMarkerRequest request) => new()
    {
        Id = Guid.NewGuid(),
        Name = request.Name.Trim(),
        Category = request.Category.Trim(),
        Address = request.Address?.Trim(),
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        IsPartner = request.IsPartner,
        IsPublished = true,
        SourceUrl = request.SourceUrl?.Trim(),
        UpdatedAt = DateTimeOffset.UtcNow
    };
}
