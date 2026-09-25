using BookyServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class ProfileBookConfiguration : IEntityTypeConfiguration<ProfileBook>
{
    public void Configure(EntityTypeBuilder<ProfileBook> builder)
    {
        builder.ToTable("ProfileBooks");
        builder.HasKey(item => new { item.ProfileId, item.BookId });
        builder.Property(item => item.Rating).HasConversion<string>().HasMaxLength(24);
        builder.Property(item => item.ReadAt).HasColumnType("date");
        builder.HasOne(item => item.Book).WithMany(book => book.Profiles).HasForeignKey(item => item.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
