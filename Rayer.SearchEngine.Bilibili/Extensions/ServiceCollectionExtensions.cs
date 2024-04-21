using Microsoft.Extensions.DependencyInjection;

namespace Rayer.SearchEngine.Bilibili.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBilibili(this IServiceCollection services)
    {
        services.UseAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}