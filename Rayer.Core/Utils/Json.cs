using System.IO;

namespace Rayer.Core.Utils;

public static class Json<T>
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public static T LoadData(string filePath)
    {
        using var file = File.OpenRead(filePath);

        var tmp = JsonSerializer.Deserialize<T>(file, _jsonSerializerOptions)!;

        return tmp is not null
            ? tmp
            : throw new ArgumentNullException(nameof(filePath), "json null/corrupt");
    }

    public static void StoreData(string filePath, T data)
    {
        if (Path.GetDirectoryName(filePath) is string path && !Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);

        JsonSerializer.Serialize(stream, data, _jsonSerializerOptions);
    }
}