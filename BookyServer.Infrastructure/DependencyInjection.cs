using BookyServer.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookyServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookyInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(Constants.Configuration.BookyDatabaseConnectionString)
            ?? throw new InvalidOperationException(Constants.Configuration.MissingBookyDatabaseConnectionString);

        services.AddDbContext<BookyServerDbContext>(options => options.UseSqlServer(
            connectionString,
            sql => sql.MigrationsAssembly(typeof(BookyServerDbContext).Assembly.FullName!)));

        return services;
    }
}
