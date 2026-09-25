using BookyServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class MapMarkerConfiguration : IEntityTypeConfiguration<MapMarker>
{
    public void Configure(EntityTypeBuilder<MapMarker> builder)
    {
        builder.ToTable("MapMarkers", table =>
        {
            table.HasCheckConstraint("CK_MapMarkers_Latitude", "[Latitude] >= -90 AND [Latitude] <= 90");
            table.HasCheckConstraint("CK_MapMarkers_Longitude", "[Longitude] >= -180 AND [Longitude] <= 180");
        });
        builder.HasKey(marker => marker.Id);
        builder.Property(marker => marker.Name).HasMaxLength(160).IsRequired();
        builder.Property(marker => marker.Category).HasMaxLength(40).IsRequired();
        builder.Property(marker => marker.Address).HasMaxLength(300);
        builder.Property(marker => marker.SourceUrl).HasMaxLength(2048);
        builder.HasIndex(marker => new { marker.IsPublished, marker.Category });
    }
}
