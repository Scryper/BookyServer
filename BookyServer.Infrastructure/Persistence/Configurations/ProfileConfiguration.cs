using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable(Constants.Database.ProfilesTable);
        builder.HasKey(profile => profile.Id);

        builder.HasIndex(profile => profile.UserId)
               .IsUnique();

        builder.Property(profile => profile.FirstName)
               .HasMaxLength(80)
               .IsRequired();

        builder.Property(profile => profile.Bio)
               .HasMaxLength(1000);

        builder.Property(profile => profile.City)
               .HasMaxLength(120);

        builder.Property(profile => profile.BirthDate)
               .HasColumnType(Constants.Database.DateColumnType);

        builder.HasOne<BookyUser>()
               .WithOne()
               .HasForeignKey<Profile>(profile => profile.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(profile => profile.Interests)
               .WithOne()
               .HasForeignKey(interest => interest.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(profile => profile.Books)
               .WithOne(book => book.Profile)
               .HasForeignKey(book => book.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(profile => profile.Photos)
               .WithOne()
               .HasForeignKey(photo => photo.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
