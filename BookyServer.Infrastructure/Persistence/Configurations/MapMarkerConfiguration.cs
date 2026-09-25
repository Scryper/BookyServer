using BookyServer.Domain.Entities;
using BookyServer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class MapMarkerConfiguration : IEntityTypeConfiguration<MapMarker>
{
    public void Configure(EntityTypeBuilder<MapMarker> builder)
    {
        builder.ToTable(Constants.Database.MapMarkersTable, table =>
        {
            table.HasCheckConstraint(Constants.Database.MapMarkerLatitudeConstraint, Constants.Database.MapMarkerLatitudePredicate);
            table.HasCheckConstraint(Constants.Database.MapMarkerLongitudeConstraint, Constants.Database.MapMarkerLongitudePredicate);
        });
        builder.HasKey(marker => marker.Id);
        builder.Property(marker => marker.Name).HasMaxLength(160).IsRequired();
        builder.Property(marker => marker.Category).HasMaxLength(40).IsRequired();
        builder.Property(marker => marker.Address).HasMaxLength(300);
        builder.Property(marker => marker.SourceUrl).HasMaxLength(2048);
        builder.HasIndex(marker => new { marker.IsPublished, marker.Category });
    }
}
