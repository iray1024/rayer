using Rayer.FFmpegCore.Modules.Abstractions;

namespace Rayer.FFmpegCore.Modules;

internal sealed class TimeConverterFactory
{
    private static readonly TimeConverterFactory _instance = new();

    public static TimeConverterFactory Instance => _instance;

    private readonly Dictionary<Type, TimeConverter> _timeConverters;
    private readonly Dictionary<Type, CacheItem> _cache;

    private TimeConverterFactory()
    {
        _timeConverters = [];
        _cache = [];

        RegisterTimeConverterForSourceType<IWaveSource>(TimeConverter.WaveSourceTimeConverter);
        RegisterTimeConverterForSourceType<ISampleSource>(TimeConverter.SampleSourceTimeConverter);
    }

    public void RegisterTimeConverterForSourceType<TSource>(TimeConverter timeConverter)
            where TSource : IAudioSource
    {
        ArgumentNullException.ThrowIfNull(timeConverter);

        var type = typeof(TSource);
        if (_timeConverters.ContainsKey(type))
        {
            throw new ArgumentException("A timeconverter for the same source type got already registered.");
        }

        _timeConverters.Add(type, timeConverter);
    }

    public void UnregisterTimeConverter<TSource>()
            where TSource : IAudioSource
    {
        var type = typeof(TSource);
        if (!_timeConverters.ContainsKey(type))
        {
            throw new ArgumentException("There is no timeconverter registered for the specified source type.");
        }

        _timeConverters.Remove(type);
    }

    public TimeConverter GetTimeConverterForSource<TSource>(TSource source) where TSource : class, IAudioSource
    {
        return source == null ? throw new ArgumentNullException(nameof(source)) : GetTimeConverterForSourceType(source.GetType());
    }

    public TimeConverter GetTimeConverterForSource<TSource>()
            where TSource : IAudioSource
    {
        return GetTimeConverterForSourceType(typeof(TSource));
    }

    public TimeConverter GetTimeConverterForSourceType(Type sourceType)
    {
        ArgumentNullException.ThrowIfNull(sourceType);

        if (!typeof(IAudioSource).IsAssignableFrom(sourceType))
        {
            throw new ArgumentException("Specified type is no AudioSource.", nameof(sourceType));
        }

        if (_cache.ContainsKey(sourceType))
        {
            return _cache[sourceType].GetTimeConverter();
        }

        var attribute =
            sourceType.GetCustomAttributes(typeof(TimeConverterAttribute), false).FirstOrDefault() as
                TimeConverterAttribute;

        TimeConverter timeConverter = null!;
        try
        {
            if (attribute == null)
            {
                var baseTypes = GetTypes(sourceType).Where(_timeConverters.ContainsKey).ToArray();

                if (baseTypes.Length == 1)
                {
                    timeConverter = _timeConverters[baseTypes.First()];
                    return timeConverter;
                }

                if (baseTypes.Length == 0)
                {
                    throw new ArgumentException(
                        "No registered time converter for the specified source type was found.");
                }

                throw new ArgumentException(
                    "Multiple possible time converters, for the specified source type, were found. Specify which time converter to use, through the TimeConverterAttribute.");
            }

            var timeConverterType = attribute.TimeConverterType;
            timeConverter = (TimeConverter)Activator.CreateInstance(timeConverterType, attribute.Args)!;

            return timeConverter;
        }
        finally
        {

            if (timeConverter != null)
            {
                var cacheItem = attribute == null
                    ? new CacheItem { CreateNewInstance = false, TimeConverter = timeConverter }
                    : new CacheItem
                    {
                        CreateNewInstance = attribute.ForceNewInstance,
                        TimeConverterAttribute = attribute,
                        TimeConverter = attribute.ForceNewInstance ? null! : timeConverter
                    };
                _cache[sourceType] = cacheItem;
            }
        }
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    private static IEnumerable<Type> GetTypes(Type type)
    {
        return type.BaseType == typeof(object)
            ? type.GetInterfaces()
            : type.BaseType is not null
                ? Enumerable
                    .Repeat(type.BaseType, 1)
                    .Concat(type.GetInterfaces())
                    .Concat(GetTypes(type.BaseType))
                    .Distinct()
                : [];
    }

    private class CacheItem
    {
        public TimeConverter TimeConverter { get; set; } = null!;

        public TimeConverterAttribute TimeConverterAttribute { get; set; } = null!;

        public bool CreateNewInstance { get; set; }

        public TimeConverter GetTimeConverter()
        {
            return CreateNewInstance
                ? (TimeConverter)Activator.CreateInstance(TimeConverterAttribute.TimeConverterType, TimeConverterAttribute.Args)!
                : TimeConverter;
        }
    }
}