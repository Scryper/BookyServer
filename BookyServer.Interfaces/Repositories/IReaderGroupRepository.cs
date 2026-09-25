using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Interfaces.Repositories;

public interface IReaderGroupRepository
{
    Task<IReadOnlyList<ReaderGroup>> GetActiveAsync(CancellationToken cancellationToken);
    Task<ReaderGroup?> GetByIdAsync(Guid groupId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReaderGroupMemberDto>> GetMembersAsync(Guid groupId, CancellationToken cancellationToken);
    Task<JoinGroupResult> TryJoinAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
    Task RemoveMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
}
