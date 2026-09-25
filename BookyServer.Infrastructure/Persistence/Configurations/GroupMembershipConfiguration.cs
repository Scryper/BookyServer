using BookyServer.Domain.Entities;
using BookyServer.Infrastructure;
using BookyServer.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class GroupMembershipConfiguration : IEntityTypeConfiguration<GroupMembership>
{
    public void Configure(EntityTypeBuilder<GroupMembership> builder)
    {
        builder.ToTable(Constants.Database.GroupMembershipsTable);
        builder.HasKey(member => new { member.GroupId, member.UserId });
        builder.HasOne<BookyUser>().WithMany().HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(member => member.UserId);
    }
}
