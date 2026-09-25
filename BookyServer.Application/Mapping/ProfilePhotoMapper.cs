using BookyServer.Domain.Entities;
using BookyServer.Interfaces.Contracts;

namespace BookyServer.Application.Mapping;

internal static class ProfilePhotoMapper
{
    public static ProfilePhotoDto Map(ProfilePhoto photo)
    {
        return new ProfilePhotoDto(photo.Id, photo.Category.ToString(), photo.SortOrder);
    }
}
