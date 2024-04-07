using System.ComponentModel;
using System.Reflection;

namespace Rayer.Core.Utils;

public static class EnumHelper
{
    public static TEnum ParseEnum<TEnum>(string description)
        where TEnum : struct, Enum
    {
        var fields = typeof(TEnum).GetFields();

        foreach (var field in fields)
        {
            var descriptionAttr = field.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttr is not null && descriptionAttr.Description == description)
            {
                return Enum.Parse<TEnum>(field.Name);
            }
        }

        return default!;
    }

    public static string GetDescription<T>(T @enum)
    {
        if (@enum is not null)
        {
            var type = typeof(T);

            var fieldContent = @enum.ToString()!;
            var field = type.GetField(fieldContent);

            var ev = field?.GetCustomAttribute<DescriptionAttribute>();

            return ev is null ? fieldContent : ev.Description;
        }

        return default!;
    }
}