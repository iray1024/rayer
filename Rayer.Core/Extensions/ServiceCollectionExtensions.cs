using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Internal;
using Rayer.Core.Framework.Injection;
using System.IO;
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

    public static IServiceCollection UseAutoMapper(this IServiceCollection services)
    {
        var projectPath = AppContext.BaseDirectory;

        var assemblies = Directory
            .GetFiles(projectPath, "Rayer*.dll")
            .Select(Assembly.LoadFrom)
            .ToArray();

        services.AddAutoMapper(cfg => cfg.AddExpressionMapping(), assemblies);

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
            var assembly = Assembly.LoadFrom(path);

            assembly
                .GetTypes()
                .Where(x => x.IsDefined(typeof(InjectAttribute), true))
                .ToList()
                .ForEach(x =>
                {
                    var attrs = x.GetCustomAttributes<InjectAttribute>(true);

                    foreach (var attr in attrs)
                    {
                        if (attr is not null)
                        {
                            var type = attr.GetType();

                            var interfaceType = type.IsGenericType ? type.GenericTypeArguments[0] : null;

                            Inject(services, interfaceType, type, attr);
                        }
                    }
                });
        }

        return services;
    }

    private static void Inject(IServiceCollection services, Type? interfaceType, Type serviceType, InjectAttribute attr)
    {
        switch (attr.ServiceLifetime)
        {
            case ServiceLifetime.Singleton:
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
                break;
            case ServiceLifetime.Scoped:
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
                break;
            case ServiceLifetime.Transient:
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
                break;
            default:
                break;
        }
    }
}