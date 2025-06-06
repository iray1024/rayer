using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Rayer.FrameworkCore.Injection;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Rayer.FrameworkCore;

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
                    !x.IsDefined(typeof(InjectAttribute), true) &&
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
        services.AddMemoryCache(options =>
        {
            options.Clock = new SystemClock();
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
            options.CompactionPercentage = .15f;
            options.TrackLinkedCacheEntries = true;
        });

        var projectPath = AppContext.BaseDirectory;

        var assemblies = Directory.GetFiles(projectPath, "Rayer*.dll");

        foreach (var path in assemblies)
        {
            var injectionTypes = AssemblyLoadContext.Default.LoadFromAssemblyPath(path)
                .DefinedTypes
                .Where(x =>
                    x.IsClass &&
                    !x.IsAbstract &&
                    x.IsDefined(typeof(InjectAttribute), true));

            foreach (var injectionType in injectionTypes)
            {
                if (injectionType.GetCustomAttributes<InjectAttribute>(true) is IEnumerable<InjectAttribute> attrs)
                {
                    foreach (var attr in attrs)
                    {
                        if (attr is not null)
                        {
                            var type = attr.GetType();

                            var interfaceType = type.IsGenericType ? type.GenericTypeArguments[0] : null;

                            Inject(services, interfaceType, injectionType, attr);
                        }
                    }
                }
            }
        }

        return services;
    }

    private static void Inject(IServiceCollection services, Type? interfaceType, Type serviceType, InjectAttribute attr)
    {
        switch (attr.ServiceLifetime)
        {
            case ServiceLifetime.Singleton:
                InjectSingleton(services, interfaceType, serviceType, attr);
                break;
            case ServiceLifetime.Scoped:
                InjectScoped(services, interfaceType, serviceType, attr);
                break;
            case ServiceLifetime.Transient:
                InjectTransient(services, interfaceType, serviceType, attr);
                break;
            default:
                break;
        }
    }

    #region Internal
    private static void InjectSingleton(IServiceCollection services, Type? interfaceType, Type serviceType, InjectAttribute attr)
    {
        if (attr.ServiceKey is null)
        {
            if (interfaceType is not null)
            {
                services.AddSingleton(interfaceType, serviceType);

                if (attr.ResolveServiceType)
                {
                    services.AddSingleton(serviceType, sp => sp.GetRequiredService(interfaceType));
                }
            }
            else
            {
                services.AddSingleton(serviceType);
            }
        }
        else
        {
            if (interfaceType is not null)
            {
                services.AddKeyedSingleton(interfaceType, attr.ServiceKey, serviceType);

                if (attr.ResolveServiceType)
                {
                    services.AddKeyedSingleton(serviceType, attr.ServiceKey, (sp, k) => sp.GetRequiredKeyedService(interfaceType, k));
                }
            }
            else
            {
                services.AddKeyedSingleton(serviceType, attr.ServiceKey);
            }
        }
    }

    private static void InjectScoped(IServiceCollection services, Type? interfaceType, Type serviceType, InjectAttribute attr)
    {
        if (attr.ServiceKey is null)
        {
            if (interfaceType is not null)
            {
                services.AddScoped(interfaceType, serviceType);

                if (attr.ResolveServiceType)
                {
                    services.AddScoped(serviceType, sp => sp.GetRequiredService(interfaceType));
                }
            }
            else
            {
                services.AddScoped(serviceType);
            }
        }
        else
        {
            if (interfaceType is not null)
            {
                services.AddKeyedScoped(interfaceType, attr.ServiceKey, serviceType);

                if (attr.ResolveServiceType)
                {
                    services.AddKeyedScoped(serviceType, attr.ServiceKey, (sp, k) => sp.GetRequiredKeyedService(interfaceType, k));
                }
            }
            else
            {
                services.AddKeyedScoped(serviceType, attr.ServiceKey);
            }
        }
    }

    private static void InjectTransient(IServiceCollection services, Type? interfaceType, Type serviceType, InjectAttribute attr)
    {
        if (attr.ServiceKey is null)
        {
            if (interfaceType is not null)
            {
                services.AddTransient(interfaceType, serviceType);

                if (attr.ResolveServiceType)
                {
                    services.AddTransient(serviceType, sp => sp.GetRequiredService(interfaceType));
                }
            }
            else
            {
                services.AddTransient(serviceType);
            }
        }
        else
        {
            if (interfaceType is not null)
            {
                services.AddKeyedTransient(interfaceType, attr.ServiceKey, serviceType);

                if (attr.ResolveServiceType)
                {
                    services.AddKeyedTransient(serviceType, attr.ServiceKey, (sp, k) => sp.GetRequiredKeyedService(interfaceType, k));
                }
            }
            else
            {
                services.AddKeyedTransient(serviceType, attr.ServiceKey);
            }
        }
    }
    #endregion
}