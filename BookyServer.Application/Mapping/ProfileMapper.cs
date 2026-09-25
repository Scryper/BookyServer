using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class ProfileMapper
{
    public static ProfileDto Map(Profile profile)
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
                .Select(ProfileBookMapper.Map)
                .ToArray(),
            profile.Photos.OrderBy(photo => photo.Category).ThenBy(photo => photo.SortOrder)
                .Select(ProfilePhotoMapper.Map)
                .ToArray());
    }

    internal static string ToRatingCode(BookRating rating)
    {
        return rating switch
        {
            BookRating.Detested => Constants.Ratings.Detested,
            BookRating.Disliked => Constants.Ratings.Disliked,
            BookRating.Liked => Constants.Ratings.Liked,
            BookRating.Favorite => Constants.Ratings.Favorite,
            _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, Constants.Errors.UnknownBookRating)
        };
    }
}
