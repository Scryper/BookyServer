using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Persistence;
using BookyServer.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence.Repositories;

internal sealed class MapMarkerRepository(BookyServerDbContext db) : IMapMarkerRepository
{
    public async Task<IReadOnlyList<MapMarker>> GetPublishedAsync(CancellationToken cancellationToken) =>
        await db.MapMarkers.AsNoTracking()
            .Where(marker => marker.IsPublished)
            .OrderBy(marker => marker.Name)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(MapMarker marker, CancellationToken cancellationToken)
    {
        db.MapMarkers.Add(marker);
        await db.SaveChangesAsync(cancellationToken);
    }
}
