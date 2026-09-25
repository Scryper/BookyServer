using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Persistence;
using BookyServer.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence.Repositories;

internal sealed class ConversationRepository(BookyServerDbContext db) : IConversationRepository
{
    private readonly BookyServerDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    public async Task<bool> IsMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        return await this._db.GroupMemberships.AsNoTracking()
            .AnyAsync(member => member.GroupId == groupId && member.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<ConversationMessage>> GetMessagesAsync(
        Guid groupId, int limit, CancellationToken cancellationToken)
    {
        return await this._db.ConversationMessages.AsNoTracking()
            .Where(message => message.GroupId == groupId)
            .OrderByDescending(message => message.CreatedAt)
            .Take(limit)
            .OrderBy(message => message.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMessageAsync(ConversationMessage message, CancellationToken cancellationToken)
    {
        this._db.ConversationMessages.Add(message);
        await this._db.SaveChangesAsync(cancellationToken);
    }
}
