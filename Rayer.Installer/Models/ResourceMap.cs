using System.IO;

namespace Rayer.Installer.Models;

public class ResourceMap
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string DestinationDirectory { get; set; } = string.Empty;

    public bool IsReplace { get; set; } = true;

    public Stream? ResourceStream { get; set; }
}