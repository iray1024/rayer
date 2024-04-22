using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Extensions;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.SearchEngine.Core.Options;
using System.Reflection;

namespace Rayer.SearchEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSearchEngine(this IServiceCollection services, Action<SearchEngineOptionsBuilder> builder)
    {
        var builderInstance = new SearchEngineOptionsBuilder(new SearchEngineOptions());
        builder(builderInstance);

        services.AddOptions<SearchEngineOptions>()
            .Configure<ISettingsService, IServiceProvider>((options, settings, provider) =>
            {
                var instanceOptions = builderInstance.Build();

                options.HttpEndpoint = instanceOptions.HttpEndpoint;
                options.SearcherType = settings.Settings.DefaultSearcher;
                options.ServiceProvider = provider;
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