using BookyServer.Application.Mapping;
using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class ProfileService(IProfileRepository profiles) : IProfileService
{
    private readonly IProfileRepository _profiles = profiles ?? throw new ArgumentNullException(nameof(profiles));

    public async Task<ProfileDto?> GetMineAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await this._profiles.GetByUserIdAsync(userId, cancellationToken);
        return profile is null ? null : ProfileMapper.Map(profile);
    }

    public async Task<ProfileDto?> GetByIdAsync(
        Guid profileId, Guid? viewerUserId, CancellationToken cancellationToken)
    {
        var profile = await this._profiles.GetByIdAsync(profileId, cancellationToken);
        if (profile is null || (!profile.IsPublic && profile.UserId != viewerUserId))
        {
            return null;
        }

        return ProfileMapper.Map(profile);
    }

    public async Task<ProfileDto> UpdateMineAsync(
        Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Trim().Length > 80)
        {
            throw new ArgumentException(Constants.Errors.InvalidProfileFirstName);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.BirthDate > today || request.BirthDate < today.AddYears(-120))
        {
            throw new ArgumentException(Constants.Errors.InvalidProfileBirthDate);
        }

        if (request.ReadingInterests.Count + request.OtherInterests.Count > 20)
        {
            throw new ArgumentException(Constants.Errors.TooManyProfileInterests);
        }

        var profile = await this._profiles.GetByUserIdAsync(userId, cancellationToken)
            ?? new Profile { Id = Guid.NewGuid(), UserId = userId };

        profile.FirstName = request.FirstName.Trim();
        profile.BirthDate = request.BirthDate;
        profile.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio.Trim();
        profile.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        profile.IsPublic = request.IsPublic;
        profile.HasVehicle = request.HasVehicle;
        profile.OffersCarpool = request.OffersCarpool;
        profile.SeeksCarpool = request.SeeksCarpool;
        profile.UpdatedAt = DateTimeOffset.UtcNow;

        profile.Interests.Clear();
        AddInterests(profile, request.ReadingInterests, isReadingInterest: true);
        AddInterests(profile, request.OtherInterests, isReadingInterest: false);

        await this._profiles.SaveAsync(profile, cancellationToken);
        return ProfileMapper.Map(profile);
    }

    public async Task<IReadOnlyList<BookCatalogDto>> SearchBooksAsync(
        string? query, int limit, CancellationToken cancellationToken)
    {
        var books = await this._profiles.SearchBooksAsync(query, Math.Clamp(limit, 1, 100), cancellationToken);
        return books.Select(BookCatalogMapper.Map).ToArray();
    }

    public async Task<ProfileBookDto?> RateBookAsync(
        Guid userId, Guid bookId, RateProfileBookRequest request, CancellationToken cancellationToken)
    {
        var rating = request.Rating.Trim().ToLowerInvariant() switch
        {
            Constants.Ratings.Detested => BookRating.Detested,
            Constants.Ratings.Disliked => BookRating.Disliked,
            Constants.Ratings.Liked => BookRating.Liked,
            Constants.Ratings.Favorite => BookRating.Favorite,
            _ => throw new ArgumentException(Constants.Errors.InvalidBookRating)
        };

        var result = await this._profiles.SetBookRatingAsync(
            userId, bookId, rating, request.ReadAt, cancellationToken);
        return result is null ? null : ProfileBookMapper.Map(result);
    }

    private static void AddInterests(Profile profile, IReadOnlyList<string> values, bool isReadingInterest)
    {
        foreach (var item in values.Where(item => !string.IsNullOrWhiteSpace(item))
                     .Select(item => item.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (item.Length > 60)
            {
                throw new ArgumentException(Constants.Errors.InvalidProfileInterest);
            }

            profile.Interests.Add(new ProfileInterest
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
                Name = item,
                IsReadingInterest = isReadingInterest
            });
        }
    }
}
