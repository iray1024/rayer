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

    public static IServiceCollection AddRayerCore(this IServiceCollection services)
    {
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

                            switch (attr.ServiceLifetime)
                            {
                                case ServiceLifetime.Singleton:
                                    _ = interfaceType is not null ? services.AddSingleton(interfaceType, x) : services.AddSingleton(x);
                                    break;
                                case ServiceLifetime.Scoped:
                                    _ = interfaceType is not null ? services.AddScoped(interfaceType, x) : services.AddScoped(x);
                                    break;
                                case ServiceLifetime.Transient:
                                    _ = interfaceType is not null ? services.AddTransient(interfaceType, x) : services.AddTransient(x);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                });
        }

        return services;
    }
}