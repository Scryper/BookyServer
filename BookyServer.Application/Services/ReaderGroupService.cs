using BookyServer.Application.Mapping;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class ReaderGroupService(IReaderGroupRepository groups) : IReaderGroupService
{
    public async Task<IReadOnlyList<ReaderGroupDto>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var result = await groups.GetActiveAsync(cancellationToken);
        return result.Select(ReaderGroupMapper.ToDto).ToArray();
    }

    public async Task<IReadOnlyList<ReaderGroupMemberDto>?> GetMembersAsync(
        Guid groupId, CancellationToken cancellationToken)
    {
        if (await groups.GetByIdAsync(groupId, cancellationToken) is null)
        {
            return null;
        }

        return await groups.GetMembersAsync(groupId, cancellationToken);
    }

    public async Task<JoinGroupResult?> JoinAsync(
        Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        if (await groups.GetByIdAsync(groupId, cancellationToken) is null)
        {
            return null;
        }

        return await groups.TryJoinAsync(groupId, userId, cancellationToken);
    }

    public async Task<bool> LeaveAsync(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        if (await groups.GetByIdAsync(groupId, cancellationToken) is null)
        {
            return false;
        }

        await groups.RemoveMemberAsync(groupId, userId, cancellationToken);
        return true;
    }
}
