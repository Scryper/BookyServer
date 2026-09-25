using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Persistence;
using BookyServer.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence.Repositories;

internal sealed class MapMarkerRepository(BookyServerDbContext db) : IMapMarkerRepository
{
    private readonly BookyServerDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    public async Task<IReadOnlyList<MapMarker>> GetPublishedAsync(CancellationToken cancellationToken)
    {
        return await this._db.MapMarkers.AsNoTracking()
            .Where(marker => marker.IsPublished)
            .OrderBy(marker => marker.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MapMarker marker, CancellationToken cancellationToken)
    {
        this._db.MapMarkers.Add(marker);
        await this._db.SaveChangesAsync(cancellationToken);
    }
}
