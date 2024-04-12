using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Abstractions;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Internal;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Internal.ApiSelector;
using Rayer.SearchEngine.Login.Abstractions;
using Rayer.SearchEngine.Login.Services;
using Rayer.SearchEngine.Lyric.Extensions;
using Rayer.SearchEngine.Services;
using System.Reflection;

namespace Rayer.SearchEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSearchEngine(this IServiceCollection services, Action<SearchEngineOptionsBuilder> builder)
    {
        services.AddSingleton<IRequestService, RequestService>();
        services.AddSingleton<ICookieManager, CookieManager>();
        services.AddScoped<IPhoneService, PhoneService>();
        services.AddScoped<IQrCodeService, QrCodeService>();
        services.AddScoped<IAnonymousService, AnonymousService>();

        services.AddSingleton<ILoginManager, LoginManager>();

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

        services.AddApiSelector();
        services.AddLyricSearch();
        services.AddDynamicIsland();

        return services;
    }

    private static IServiceCollection AddApiSelector(this IServiceCollection services)
    {
        services.AddSingleton<AccountApiSelector>();
        services.AddSingleton<LoginApiSelector>();
        services.AddSingleton<LyricApiSelector>();
        services.AddSingleton<SearchApiSelector>();
        services.AddSingleton<TrackApiSelector>();

        return services;
    }
}