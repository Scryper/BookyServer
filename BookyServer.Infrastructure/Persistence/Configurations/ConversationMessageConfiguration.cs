using BookyServer.Domain.Entities;
using BookyServer.Infrastructure;
using BookyServer.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyServer.Infrastructure.Persistence.Configurations;

internal sealed class ConversationMessageConfiguration : IEntityTypeConfiguration<ConversationMessage>
{
    public void Configure(EntityTypeBuilder<ConversationMessage> builder)
    {
        builder.ToTable(Constants.Database.ConversationMessagesTable);
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Text).HasMaxLength(3000).IsRequired();
        builder.HasOne<BookyUser>().WithMany().HasForeignKey(message => message.AuthorUserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(message => new { message.GroupId, message.CreatedAt });
    }
}
