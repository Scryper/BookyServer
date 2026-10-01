using Microsoft.Extensions.DependencyInjection;

namespace BookyServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBookyApplication(this IServiceCollection services)
    {
        return services;
    }
}
