using BookyServer.Interfaces.Contracts;

namespace BookyServer.Interfaces.Services;

public interface IMapMarkerService
{
    Task<IReadOnlyList<MapMarkerDto>> GetPublishedAsync(CancellationToken cancellationToken);
    Task<MapMarkerDto> CreateAsync(CreateMapMarkerRequest request, CancellationToken cancellationToken);
}
