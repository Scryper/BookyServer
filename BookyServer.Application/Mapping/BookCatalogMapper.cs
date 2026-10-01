using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class BookCatalogMapper
{
    public static BookCatalogDto Map(Book book)
    {
        return new BookCatalogDto(
            book.Id,
            book.Title,
            book.Author,
            book.PublicationYear,
            book.Genre,
            book.Synopsis,
            book.Isbn13,
            book.CoverUrl);
    }
}
