using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence;

public sealed class BookyServerDbContext(
    DbContextOptions<BookyServerDbContext> options)
    : IdentityDbContext<BookyUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<ProfileInterest> ProfileInterests => Set<ProfileInterest>();
    public DbSet<ProfilePhoto> ProfilePhotos => Set<ProfilePhoto>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<ProfileBook> ProfileBooks => Set<ProfileBook>();
    public DbSet<ReaderGroup> ReaderGroups => Set<ReaderGroup>();
    public DbSet<GroupMembership> GroupMemberships => Set<GroupMembership>();
    public DbSet<MapMarker> MapMarkers => Set<MapMarker>();
    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(BookyServerDbContext).Assembly);
        builder.Entity<Book>().HasData(BookCatalogSeed.All);
    }
}
