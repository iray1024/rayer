using Microsoft.Extensions.DependencyInjection;

namespace Rayer.FrameworkCore.Injection;

[AttributeUsage(AttributeTargets.Class)]
public class InjectAttribute<T> : InjectAttribute
    where T : class
{

}

[AttributeUsage(AttributeTargets.Class)]
public class InjectAttribute : Attribute
{
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Singleton;

    public bool ResolveServiceType { get; set; } = false;

    public object? ServiceKey { get; set; }
}