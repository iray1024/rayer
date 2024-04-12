namespace Rayer.SearchEngine.Extensions;

internal static class StringExtensions
{
    internal static readonly JsonSerializerOptions _jsonSerializerOptions = new();

    static StringExtensions()
    {
        _jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        _jsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    }

    public static T? ToEntity<T>(this string val) => JsonSerializer.Deserialize<T>(val, _jsonSerializerOptions);

    public static List<T>? ToEntityList<T>(this string val) => JsonSerializer.Deserialize<List<T>>(val, _jsonSerializerOptions);

    public static string? ToJson<T>(this T entity) => JsonSerializer.Serialize(entity, _jsonSerializerOptions);
}