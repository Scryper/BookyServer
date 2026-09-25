using BookyServer.Domain.Entities;
using BookyServer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable(Constants.Database.BooksTable);
        builder.HasKey(book => book.Id);
        builder.Property(book => book.Title).HasMaxLength(300).IsRequired();
        builder.Property(book => book.Author).HasMaxLength(240).IsRequired();
        builder.Property(book => book.Genre).HasMaxLength(100);
        builder.Property(book => book.Synopsis).HasMaxLength(500);
        builder.Property(book => book.Isbn13).HasMaxLength(13);
        builder.Property(book => book.CoverUrl).HasMaxLength(2048);
        builder.Property(book => book.Source).HasMaxLength(80);
        builder.Property(book => book.ExternalId).HasMaxLength(200);
        builder.HasIndex(book => book.Isbn13).IsUnique().HasFilter(Constants.Database.BookIsbnFilter);
        builder.HasIndex(book => new { book.Source, book.ExternalId }).IsUnique()
            .HasFilter(Constants.Database.BookSourceAndExternalIdFilter);
    }
}
