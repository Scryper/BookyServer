using BookyServer.Application.Mapping;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class MapMarkerService(IMapMarkerRepository markers) : IMapMarkerService
{
    public async Task<IReadOnlyList<MapMarkerDto>> GetPublishedAsync(CancellationToken cancellationToken)
    {
        var result = await markers.GetPublishedAsync(cancellationToken);
        return result.Select(MapMarkerMapper.ToDto).ToArray();
    }

    public async Task<MapMarkerDto> CreateAsync(
        CreateMapMarkerRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 160)
        {
            throw new ArgumentException("Le nom du lieu est obligatoire et doit faire au plus 160 caractères.");
        }

        if (string.IsNullOrWhiteSpace(request.Category) || request.Category.Trim().Length > 40)
        {
            throw new ArgumentException("La catégorie du lieu est obligatoire.");
        }

        if (request.Latitude is < -90 or > 90 || request.Longitude is < -180 or > 180)
        {
            throw new ArgumentException("Les coordonnées géographiques sont invalides.");
        }

        var marker = MapMarkerMapper.ToEntity(request);
        await markers.AddAsync(marker, cancellationToken);
        return MapMarkerMapper.ToDto(marker);
    }
}
