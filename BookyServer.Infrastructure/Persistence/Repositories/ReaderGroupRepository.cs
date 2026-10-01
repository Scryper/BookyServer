using System.Data;

using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Persistence;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence.Repositories;

internal sealed class ReaderGroupRepository(BookyServerDbContext db) : IReaderGroupRepository
{
    private readonly BookyServerDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    public async Task<IReadOnlyList<ReaderGroup>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await this._db.ReaderGroups.AsNoTracking()
            .Where(group => group.IsActive)
            .Include(group => group.Members)
            .OrderBy(group => group.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReaderGroup?> GetByIdAsync(Guid groupId, CancellationToken cancellationToken)
    {
        return await this._db.ReaderGroups.AsNoTracking()
            .Include(group => group.Members)
            .SingleOrDefaultAsync(group => group.Id == groupId && group.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<ReaderGroupMemberDto>> GetMembersAsync(
        Guid groupId, CancellationToken cancellationToken)
    {
        return await (from member in this._db.GroupMemberships.AsNoTracking()
                      join profile in this._db.Profiles.AsNoTracking() on member.UserId equals profile.UserId into profileJoin
                      from profile in profileJoin.DefaultIfEmpty()
                      where member.GroupId == groupId
                      orderby member.JoinedAt
                      select new ReaderGroupMemberDto(
                          member.UserId,
                          profile == null ? null : profile.FirstName,
                          member.JoinedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<JoinGroupResult> TryJoinAsync(
        Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        await using var transaction = await this._db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        var group = await this._db.ReaderGroups
            .SingleOrDefaultAsync(item => item.Id == groupId && item.IsActive, cancellationToken);
        if (group is null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new JoinGroupResult(false, false);
        }

        var existing = await this._db.GroupMemberships.AnyAsync(
            item => item.GroupId == groupId && item.UserId == userId, cancellationToken);
        if (existing)
        {
            await transaction.CommitAsync(cancellationToken);
            return new JoinGroupResult(true, false);
        }

        var memberCount = await this._db.GroupMemberships.CountAsync(
            item => item.GroupId == groupId, cancellationToken);
        if (memberCount >= group.MaxMembers)
        {
            await transaction.CommitAsync(cancellationToken);
            return new JoinGroupResult(false, true);
        }

        this._db.GroupMemberships.Add(new GroupMembership
        {
            GroupId = groupId,
            UserId = userId,
            JoinedAt = DateTimeOffset.UtcNow
        });
        await this._db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new JoinGroupResult(true, false);
    }

    public async Task RemoveMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var membership = await this._db.GroupMemberships.FindAsync([groupId, userId], cancellationToken);
        if (membership is null)
        {
            return;
        }

        this._db.GroupMemberships.Remove(membership);
        await this._db.SaveChangesAsync(cancellationToken);
    }
}
