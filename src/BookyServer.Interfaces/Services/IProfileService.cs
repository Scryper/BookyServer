using BookyServer.Interfaces.Contracts;

namespace BookyServer.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileDto?> GetMineAsync(Guid userId, CancellationToken cancellationToken);
    Task<ProfileDto?> GetByIdAsync(Guid profileId, Guid? viewerUserId, CancellationToken cancellationToken);
    Task<ProfileDto> UpdateMineAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<BookCatalogDto>> SearchBooksAsync(
        string? query, int limit, CancellationToken cancellationToken);
    Task<ProfileBookDto?> RateBookAsync(
        Guid userId, Guid bookId, RateProfileBookRequest request, CancellationToken cancellationToken);
}
