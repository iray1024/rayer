using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

namespace Rayer.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseAutoMapper(this IServiceCollection services, Assembly assembly)
    {
        services.AddAutoMapper(cfg => cfg.AddExpressionMapping(), assembly);

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
}