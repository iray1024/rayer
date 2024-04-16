using ICSharpCode.SharpZipLib.Zip;
using Rayer.Installer.Models;
using System.IO;

namespace Rayer.Installer.Services;

public static class FileOperator
{
    public static async Task ExtractorAll(ResourceMap[] resources, IProgress<double> progress)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var entryConut = 0L;
        foreach (var resource in resources)
        {
            if (resource.ResourceStream is not null)
            {
                using var tempStream = new MemoryStream();

                await resource.ResourceStream.CopyToAsync(tempStream);

                using var zip = new ZipFile(tempStream, false, StringCodec.FromCodePage(StringCodec.SystemDefaultCodePage));

                entryConut += zip.Count;

                resource.ResourceStream.Position = 0;
            }
        }

        var percent = 100.0 / entryConut;

        var index = 0;

        foreach (var resource in resources)
        {
            if (!Directory.Exists(resource.DestinationDirectory))
            {
                Directory.CreateDirectory(resource.DestinationDirectory);
            }

            using var inStream = new ZipInputStream(resource.ResourceStream, StringCodec.FromCodePage(StringCodec.SystemDefaultCodePage));

            ZipEntry zipEntry;

            while ((zipEntry = inStream.GetNextEntry()) is not null)
            {
                var entryPath = Path.Combine(resource.DestinationDirectory, zipEntry.Name);
                var entryDir = Path.GetDirectoryName(entryPath);

                if (zipEntry.IsDirectory && resource.IsFolder && !resource.IsReplace)
                {
                    break;
                }

                if (!Directory.Exists(entryDir) && entryDir is not null)
                {
                    Directory.CreateDirectory(entryDir);
                }

                var fileName = Path.GetFileName(zipEntry.Name);

                if (!string.IsNullOrEmpty(fileName))
                {
                    if (File.Exists(fileName) && !resource.IsReplace)
                    {
                        continue;
                    }

                    using var outStream = File.Create(entryPath);

                    try
                    {
                        var buffer = new byte[1024];
                        var length = 0;

                        while ((length = await inStream.ReadAsync(buffer)) > 0)
                        {
                            await outStream.WriteAsync(buffer.AsMemory(0, length));
                        }

                        await outStream.FlushAsync();
                    }
                    finally
                    {
                        outStream.Close();
                    }
                }

                progress.Report(++index * percent);
            }
        }
    }

    public static void Save(Stream stream, string path)
    {
        using var inStream = new BufferedStream(stream);
        using var outStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);

        try
        {
            var buffer = new byte[1024];
            var length = 0;

            while ((length = inStream.Read(buffer)) > 0)
            {
                outStream.Write(buffer.AsSpan(0, length));
            }

            outStream.Flush();
        }
        finally
        {
            outStream?.Close();
            inStream?.Close();
        }
    }
}