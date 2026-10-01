using BookyServer.Domain.Entities;

namespace BookyServer.Interfaces.Repositories;

public interface IMapMarkerRepository
{
    Task<IReadOnlyList<MapMarker>> GetPublishedAsync(CancellationToken cancellationToken);
    Task AddAsync(MapMarker marker, CancellationToken cancellationToken);
}
