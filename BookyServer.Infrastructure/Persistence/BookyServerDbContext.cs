using BookyServer.Domain.Entities;
using BookyServer.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence;

public sealed class BookyServerDbContext : IdentityDbContext<BookyUser, IdentityRole<Guid>, Guid>
{
    public BookyServerDbContext(DbContextOptions<BookyServerDbContext> options)
        : base(options ?? throw new ArgumentNullException(nameof(options)))
    {
    }

    public DbSet<Profile> Profiles => this.Set<Profile>();
    public DbSet<ProfileInterest> ProfileInterests => this.Set<ProfileInterest>();
    public DbSet<ProfilePhoto> ProfilePhotos => this.Set<ProfilePhoto>();
    public DbSet<Book> Books => this.Set<Book>();
    public DbSet<ProfileBook> ProfileBooks => this.Set<ProfileBook>();
    public DbSet<ReaderGroup> ReaderGroups => this.Set<ReaderGroup>();
    public DbSet<GroupMembership> GroupMemberships => this.Set<GroupMembership>();
    public DbSet<MapMarker> MapMarkers => this.Set<MapMarker>();
    public DbSet<ConversationMessage> ConversationMessages => this.Set<ConversationMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(BookyServerDbContext).Assembly);
        builder.Entity<Book>().HasData(BookCatalogSeed.All);
    }
}
