using BookyServer.Interfaces.Contracts;

namespace BookyServer.Interfaces.Services;

public interface IReaderGroupService
{
    Task<IReadOnlyList<ReaderGroupDto>> GetActiveAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ReaderGroupMemberDto>?> GetMembersAsync(Guid groupId, CancellationToken cancellationToken);
    Task<JoinGroupResult?> JoinAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
    Task<bool> LeaveAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
}
