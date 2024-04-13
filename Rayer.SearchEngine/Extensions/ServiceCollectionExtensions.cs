using Microsoft.Extensions.DependencyInjection;
using Rayer.SearchEngine.Abstractions;
using System.Reflection;

namespace Rayer.SearchEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSearchEngine(this IServiceCollection services, Action<SearchEngineOptionsBuilder> builder)
    {
        var builderInstacne = new SearchEngineOptionsBuilder();

        builder(builderInstacne);

        var options = builderInstacne.Build();

        services.AddSingleton(sp =>
        {
            options.ServiceProvider = sp;

            return options;
        });

        var assembly = Assembly.GetAssembly(typeof(ServiceCollectionExtensions));

        if (assembly is not null)
        {
            services.AddTransientFromNamespace("Rayer.SearchEngine.Views", assembly);
            services.AddTransientFromNamespace("Rayer.SearchEngine.ViewModels", assembly);
        }

        return services;
    }
}