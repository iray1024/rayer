using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Extensions;

namespace Rayer.SearchEngine.Bilibili.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBilibili(this IServiceCollection services)
    {
        services.UseAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}