using Microsoft.Extensions.DependencyInjection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Impl;
using Rayer.SearchEngine.Services;
using Rayer.SearchEngine.Views.Windows;

namespace Rayer.SearchEngine.Lyric.Extensions;

internal static class IServiceCollectionsExtensions
{
    internal static IServiceCollection AddLyricSearch(this IServiceCollection services)
    {
        services.AddSingleton<ILyricSearchEngine, LyricSearchEngine>();
        services.AddSingleton<ILyricProvider, LyricProvider>();

        services.AddSingleton<Providers.Web.Kugou.Api>();
        services.AddSingleton<Providers.Web.Netease.Api>();
        services.AddSingleton<Providers.Web.QQMusic.Api>();

        return services;
    }

    internal static IServiceCollection AddDynamicIsland(this IServiceCollection services)
    {
        services.AddSingleton<DynamicIsland>();

        return services;
    }
}