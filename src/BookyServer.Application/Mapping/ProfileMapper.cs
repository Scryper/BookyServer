using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class ProfileMapper
{
    public static ProfileDto ToDto(Profile profile)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - profile.BirthDate.Year;
        if (profile.BirthDate > today.AddYears(-age))
        {
            age--;
        }

        return new ProfileDto(
            profile.Id,
            profile.FirstName,
            age,
            profile.Bio,
            profile.City,
            profile.IsPublic,
            profile.HasVehicle,
            profile.OffersCarpool,
            profile.SeeksCarpool,
            profile.Interests.Where(interest => interest.IsReadingInterest).OrderBy(interest => interest.Name)
                .Select(interest => interest.Name).ToArray(),
            profile.Interests.Where(interest => !interest.IsReadingInterest).OrderBy(interest => interest.Name)
                .Select(interest => interest.Name).ToArray(),
            profile.Books.OrderBy(book => book.Book.Title)
                .Select(book => new ProfileBookDto(
                    book.BookId, book.Book.Title, book.Book.Author, ToRatingCode(book.Rating), book.ReadAt))
                .ToArray(),
            profile.Photos.OrderBy(photo => photo.Category).ThenBy(photo => photo.SortOrder)
                .Select(photo => new ProfilePhotoDto(
                    photo.Id, photo.Category.ToString(), photo.SortOrder)).ToArray());
    }

    public static ProfileBookDto ToDto(ProfileBook book) => new(
        book.BookId, book.Book.Title, book.Book.Author, ToRatingCode(book.Rating), book.ReadAt);

    private static string ToRatingCode(BookRating rating) => rating switch
    {
        BookRating.Detested => "deteste",
        BookRating.Disliked => "pas_aime",
        BookRating.Liked => "aime",
        BookRating.Favorite => "coup_de_coeur",
        _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, "Avis de lecture inconnu.")
    };
}
