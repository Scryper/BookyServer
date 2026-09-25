using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Persistence;
using BookyServer.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence.Repositories;

internal sealed class ProfileRepository(BookyServerDbContext db) : IProfileRepository
{
    public Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        db.Profiles
            .Include(profile => profile.Interests)
            .Include(profile => profile.Books).ThenInclude(item => item.Book)
            .Include(profile => profile.Photos)
            .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);

    public Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken) =>
        db.Profiles.AsNoTracking()
            .Include(profile => profile.Interests)
            .Include(profile => profile.Books).ThenInclude(item => item.Book)
            .Include(profile => profile.Photos)
            .SingleOrDefaultAsync(profile => profile.Id == profileId, cancellationToken);

    public async Task<IReadOnlyList<Book>> SearchBooksAsync(
        string? query, int limit, CancellationToken cancellationToken)
    {
        var books = db.Books.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            books = books.Where(book => book.Title.Contains(term) || book.Author.Contains(term));
        }

        return await books.OrderBy(book => book.Title).Take(limit).ToListAsync(cancellationToken);
    }

    public async Task<ProfileBook?> SetBookRatingAsync(
        Guid userId, Guid bookId, BookRating rating, DateOnly? readAt, CancellationToken cancellationToken)
    {
        var profileId = await db.Profiles.Where(profile => profile.UserId == userId)
            .Select(profile => profile.Id).SingleOrDefaultAsync(cancellationToken);
        if (profileId == Guid.Empty || !await db.Books.AnyAsync(book => book.Id == bookId, cancellationToken))
        {
            return null;
        }

        var profileBook = await db.ProfileBooks.FindAsync([profileId, bookId], cancellationToken);
        if (profileBook is null)
        {
            profileBook = new ProfileBook
            {
                ProfileId = profileId,
                BookId = bookId,
                Rating = rating,
                ReadAt = readAt
            };
            db.ProfileBooks.Add(profileBook);
        }
        else
        {
            profileBook.Rating = rating;
            profileBook.ReadAt = readAt;
        }

        await db.SaveChangesAsync(cancellationToken);
        return await db.ProfileBooks.AsNoTracking()
            .Include(item => item.Book)
            .SingleAsync(item => item.ProfileId == profileId && item.BookId == bookId, cancellationToken);
    }

    public async Task SaveAsync(Profile profile, CancellationToken cancellationToken)
    {
        if (db.Entry(profile).State == EntityState.Detached)
        {
            db.Profiles.Add(profile);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
