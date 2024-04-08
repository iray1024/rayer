using Rayer.Core.Abstractions;
using Rayer.Core.FileSystem;
using Rayer.Core.Playing;
using Rayer.Core.Providers;
using Rayer.Core.Services;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransientFromNamespace(
        this IServiceCollection services,
        string namespaceName,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(x =>
                    x.IsClass &&
                    !x.IsAbstract &&
                    x.Namespace?.StartsWith(namespaceName, StringComparison.InvariantCultureIgnoreCase) == true);

            foreach (var type in types)
            {
                if (services.All(x => x.ServiceType != type))
                {
                    _ = services.AddTransient(type);
                }
            }
        }

        return services;
    }

    public static IServiceCollection AddRayerCore(this IServiceCollection services)
    {
        services.AddSingleton<IAudioManager, AudioManager>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IAudioFileWatcher, AudioFileWatcher>();
        services.AddSingleton<IPlayQueueProvider, PlayQueueProvider>();
        services.AddSingleton<IWaveMetadataFactory, WaveMetadataFactory>();
        services.AddSingleton<IDeviceManager, DeviceManager>();
        services.AddSingleton<IEqualizerProvider, EqualizerProvider>();
        services.AddSingleton<IPitchShiftingProviderFactory, PitchShiftingProviderFactory>();

        return services;
    }
}