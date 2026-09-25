using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class ProfileBookMapper
{
    public static ProfileBookDto Map(ProfileBook book)
    {
        return new ProfileBookDto(
            book.BookId,
            book.Book.Title,
            book.Book.Author,
            ProfileMapper.ToRatingCode(book.Rating),
            book.ReadAt);
    }
}
