using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Interfaces.Repositories;

public interface IProfileRepository
{
    Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Book>> SearchBooksAsync(string? query, int limit, CancellationToken cancellationToken);
    Task<ProfileBook?> SetBookRatingAsync(
        Guid userId, Guid bookId, BookRating rating, DateOnly? readAt, CancellationToken cancellationToken);
    Task SaveAsync(Profile profile, CancellationToken cancellationToken);
}
