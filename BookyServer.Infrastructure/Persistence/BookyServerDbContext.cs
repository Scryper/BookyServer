using BookyServer.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookyServer.Infrastructure.Persistence;

public sealed class BookyServerDbContext(DbContextOptions<BookyServerDbContext> options)
    : IdentityDbContext<BookyUser, IdentityRole<Guid>, Guid>(
        options ?? throw new ArgumentNullException(nameof(options)))
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(BookyServerDbContext).Assembly);
    }
}
