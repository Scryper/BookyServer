using BookyServer.Application.Services;
using BookyServer.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookyServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBookyApplication(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IReaderGroupService, ReaderGroupService>();
        services.AddScoped<IMapMarkerService, MapMarkerService>();
        services.AddScoped<IConversationService, ConversationService>();
        return services;
    }
}
