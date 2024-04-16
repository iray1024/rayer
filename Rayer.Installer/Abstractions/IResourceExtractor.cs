using System.IO;

namespace Rayer.Installer.Abstractions;

internal interface IResourceExtractor
{
    Stream? GetResource(string name);
}