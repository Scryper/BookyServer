using BookyServer.Application.Mapping;
using BookyServer.Interfaces.Contracts;
using BookyServer.Interfaces.Repositories;
using BookyServer.Interfaces.Services;

namespace BookyServer.Application.Services;

public sealed class ReaderGroupService(IReaderGroupRepository groups) : IReaderGroupService
{
    private readonly IReaderGroupRepository _groups = groups ?? throw new ArgumentNullException(nameof(groups));

    public async Task<IReadOnlyList<ReaderGroupDto>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var result = await this._groups.GetActiveAsync(cancellationToken);
        return result.Select(ReaderGroupMapper.Map).ToArray();
    }

    public async Task<IReadOnlyList<ReaderGroupMemberDto>?> GetMembersAsync(
        Guid groupId, CancellationToken cancellationToken)
    {
        if (await this._groups.GetByIdAsync(groupId, cancellationToken) is null)
        {
            return null;
        }

        return await this._groups.GetMembersAsync(groupId, cancellationToken);
    }

    public async Task<JoinGroupResult?> JoinAsync(
        Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        if (await this._groups.GetByIdAsync(groupId, cancellationToken) is null)
        {
            return null;
        }

        return await this._groups.TryJoinAsync(groupId, userId, cancellationToken);
    }

    public async Task<bool> LeaveAsync(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        if (await this._groups.GetByIdAsync(groupId, cancellationToken) is null)
        {
            return false;
        }

        await this._groups.RemoveMemberAsync(groupId, userId, cancellationToken);
        return true;
    }
}
