using BookyServer.Application.Mapping;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class MapMarkerService(IMapMarkerRepository markers) : IMapMarkerService
{
    private readonly IMapMarkerRepository _markers = markers ?? throw new ArgumentNullException(nameof(markers));

    public async Task<IReadOnlyList<MapMarkerDto>> GetPublishedAsync(CancellationToken cancellationToken)
    {
        var result = await this._markers.GetPublishedAsync(cancellationToken);
        return result.Select(MapMarkerMapper.Map).ToArray();
    }

    public async Task<MapMarkerDto> CreateAsync(
        CreateMapMarkerRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 160)
        {
            throw new ArgumentException(Constants.Errors.InvalidMapMarkerName);
        }

        if (string.IsNullOrWhiteSpace(request.Category) || request.Category.Trim().Length > 40)
        {
            throw new ArgumentException(Constants.Errors.InvalidMapMarkerCategory);
        }

        if (request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180)
        {
            throw new ArgumentException(Constants.Errors.InvalidMapMarkerCoordinates);
        }

        var marker = MapMarkerMapper.Map(request);
        await this._markers.AddAsync(marker, cancellationToken);
        return MapMarkerMapper.Map(marker);
    }
}
