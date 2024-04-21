using Microsoft.Extensions.DependencyInjection;

namespace Rayer.SearchEngine.Netease.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseNetease(this IServiceCollection services)
    {
        services.UseAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}