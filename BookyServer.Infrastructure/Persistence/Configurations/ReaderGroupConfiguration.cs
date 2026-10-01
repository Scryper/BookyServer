using BookyServer.Domain.Entities;
using BookyServer.Infrastructure;
using BookyServer.Infrastructure.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class ReaderGroupConfiguration : IEntityTypeConfiguration<ReaderGroup>
{
    public void Configure(EntityTypeBuilder<ReaderGroup> builder)
    {
        builder.ToTable(Constants.Database.ReaderGroupsTable, table => table.HasCheckConstraint(
            Constants.Database.ReaderGroupMaximumMembersConstraint,
            Constants.Database.ReaderGroupMaximumMembersPredicate));
        builder.HasKey(group => group.Id);
        builder.Property(group => group.Name).HasMaxLength(120).IsRequired();
        builder.Property(group => group.Topic).HasMaxLength(160).IsRequired();
        builder.Property(group => group.City).HasMaxLength(120);
        builder.HasMany(group => group.Members)
               .WithOne(member => member.Group)
               .HasForeignKey(member => member.GroupId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(group => group.Messages)
               .WithOne(message => message.Group)
               .HasForeignKey(message => message.GroupId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
