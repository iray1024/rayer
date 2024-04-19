using Rayer.FFmpegCore.Modules.Abstractions;

namespace Rayer.FFmpegCore.Modules;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
internal sealed class TimeConverterAttribute : Attribute
{
    public TimeConverterAttribute(Type timeConverterType)
    {
        ArgumentNullException.ThrowIfNull(timeConverterType);

        if (!typeof(TimeConverter).IsAssignableFrom(timeConverterType))
        {
            throw new ArgumentException("指定的类型不是 TimeConverter。", nameof(timeConverterType));
        }

        TimeConverterType = timeConverterType;
    }

    public Type TimeConverterType { get; private set; }

    public object[] Args { get; set; } = [];

    public bool ForceNewInstance { get; set; }
}