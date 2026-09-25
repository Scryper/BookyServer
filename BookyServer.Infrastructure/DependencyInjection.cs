using BookyServer.Infrastructure.Persistence;
using BookyServer.Infrastructure.Persistence.Repositories;
using BookyServer.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookyServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookyInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BookyDatabase")
            ?? throw new InvalidOperationException("La configuration ConnectionStrings:BookyDatabase est manquante.");

        services.AddDbContext<BookyServerDbContext>(options => options.UseSqlServer(
            connectionString,
            sql => sql.MigrationsAssembly(typeof(BookyServerDbContext).Assembly.FullName!)));

        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IReaderGroupRepository, ReaderGroupRepository>();
        services.AddScoped<IMapMarkerRepository, MapMarkerRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        return services;
    }
}
