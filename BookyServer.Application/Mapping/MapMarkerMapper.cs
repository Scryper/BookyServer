using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class MapMarkerMapper
{
    public static MapMarkerDto Map(MapMarker marker)
    {
        return new MapMarkerDto(
            marker.Id,
            marker.Name,
            marker.Category,
            marker.Address,
            marker.Latitude,
            marker.Longitude,
            marker.IsPartner,
            marker.SourceUrl);
    }

    public static MapMarker Map(CreateMapMarkerRequest request)
    {
        return new MapMarker
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
}
