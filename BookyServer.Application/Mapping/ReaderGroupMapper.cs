using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class ReaderGroupMapper
{
    public static ReaderGroupDto Map(ReaderGroup group)
    {
        return new ReaderGroupDto(
            group.Id,
            group.Name,
            group.Topic,
            group.City,
            group.Members.Count,
            group.MaxMembers);
    }
}
