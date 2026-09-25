using BookyServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class ProfileInterestConfiguration : IEntityTypeConfiguration<ProfileInterest>
{
    public void Configure(EntityTypeBuilder<ProfileInterest> builder)
    {
        builder.ToTable("ProfileInterests");
        builder.HasKey(interest => interest.Id);
        builder.Property(interest => interest.Name).HasMaxLength(60).IsRequired();
        builder.HasIndex(interest => new { interest.ProfileId, interest.Name, interest.IsReadingInterest }).IsUnique();
    }
}
