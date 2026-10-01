using BookyServer.Domain.Entities;
using BookyServer.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class ProfilePhotoConfiguration : IEntityTypeConfiguration<ProfilePhoto>
{
    public void Configure(EntityTypeBuilder<ProfilePhoto> builder)
    {
        builder.ToTable(Constants.Database.ProfilePhotosTable);
        builder.HasKey(photo => photo.Id);
        builder.Property(photo => photo.Category).HasConversion<string>().HasMaxLength(24);
        builder.Property(photo => photo.ObjectKey).HasMaxLength(512).IsRequired();
        builder.HasIndex(photo => new { photo.ProfileId, photo.Category, photo.SortOrder }).IsUnique();
    }
}
