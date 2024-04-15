using System.Linq.Expressions;
using System.Reflection;

namespace Rayer.Core.Utils;

public static class TypeResolveUtils
{
    private static readonly Func<Type, Type, bool> _getDerivedTypes;

    static TypeResolveUtils()
    {
        var baseTypeParam = Expression.Parameter(typeof(Type), "baseType");
        var typeParam = Expression.Parameter(typeof(Type), "type");

        var getInterfaceMethod = typeof(Type).GetMethod(nameof(Type.GetInterface), [typeof(string)])!;

        var predicate = Expression.Lambda<Func<Type, Type, bool>>(
            Expression.NotEqual(
                Expression.Call(typeParam, getInterfaceMethod, Expression.Property(baseTypeParam, "Name")),
                Expression.Constant(null)
            ),
            baseTypeParam, typeParam
        );

        _getDerivedTypes = predicate.Compile();
    }

    public static Type[] GetDerivedTypes(Type baseType)
    {
        var derivedTypes = new List<Type>();

        foreach (var type in GetAllTypes())
        {
            if (_getDerivedTypes(baseType, type))
            {
                derivedTypes.Add(type);
            }
        }

        return [.. derivedTypes];
    }

    public static IEnumerable<Type> GetAllTypes()
    {
        var mainAssembly = Assembly.GetEntryAssembly()!;

        var name = mainAssembly.GetName().Name!;

        var referencedAssembly = mainAssembly
            .GetReferencedAssemblies()
            .Where(x => x?.Name?.Contains(name) == true);

        return referencedAssembly
            .Select(Assembly.Load)
            .Append(mainAssembly)
            .SelectMany(x => x.GetTypes());
    }
}