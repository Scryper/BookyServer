using BookyServer.Application.Mapping;
using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class ProfileService(IProfileRepository profiles) : IProfileService
{
    public async Task<ProfileDto?> GetMineAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await profiles.GetByUserIdAsync(userId, cancellationToken);
        return profile is null ? null : ProfileMapper.ToDto(profile);
    }

    public async Task<ProfileDto?> GetByIdAsync(
        Guid profileId, Guid? viewerUserId, CancellationToken cancellationToken)
    {
        var profile = await profiles.GetByIdAsync(profileId, cancellationToken);
        if (profile is null || (!profile.IsPublic && profile.UserId != viewerUserId))
        {
            return null;
        }

        return ProfileMapper.ToDto(profile);
    }

    public async Task<ProfileDto> UpdateMineAsync(
        Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Trim().Length > 80)
        {
            throw new ArgumentException("Le prénom est obligatoire et doit faire au plus 80 caractères.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.BirthDate > today || request.BirthDate < today.AddYears(-120))
        {
            throw new ArgumentException("La date de naissance est invalide.");
        }

        if (request.ReadingInterests.Count + request.OtherInterests.Count > 20)
        {
            throw new ArgumentException("Un profil peut contenir au maximum 20 centres d'intérêt.");
        }

        var profile = await profiles.GetByUserIdAsync(userId, cancellationToken)
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

        await profiles.SaveAsync(profile, cancellationToken);
        return ProfileMapper.ToDto(profile);
    }

    public async Task<IReadOnlyList<BookCatalogDto>> SearchBooksAsync(
        string? query, int limit, CancellationToken cancellationToken)
    {
        var books = await profiles.SearchBooksAsync(query, Math.Clamp(limit, 1, 100), cancellationToken);
        return books.Select(book => new BookCatalogDto(
            book.Id, book.Title, book.Author, book.PublicationYear, book.Genre,
            book.Synopsis, book.Isbn13, book.CoverUrl)).ToArray();
    }

    public async Task<ProfileBookDto?> RateBookAsync(
        Guid userId, Guid bookId, RateProfileBookRequest request, CancellationToken cancellationToken)
    {
        var rating = request.Rating.Trim().ToLowerInvariant() switch
        {
            "deteste" => BookRating.Detested,
            "pas_aime" => BookRating.Disliked,
            "aime" => BookRating.Liked,
            "coup_de_coeur" => BookRating.Favorite,
            _ => throw new ArgumentException("Avis attendu : deteste, pas_aime, aime ou coup_de_coeur.")
        };

        var result = await profiles.SetBookRatingAsync(
            userId, bookId, rating, request.ReadAt, cancellationToken);
        return result is null ? null : ProfileMapper.ToDto(result);
    }

    private static void AddInterests(Profile profile, IReadOnlyList<string> values, bool isReadingInterest)
    {
        foreach (var item in values.Where(item => !string.IsNullOrWhiteSpace(item))
                     .Select(item => item.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (item.Length > 60)
            {
                throw new ArgumentException("Chaque centre d'intérêt doit faire au plus 60 caractères.");
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
