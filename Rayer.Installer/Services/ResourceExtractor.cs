using Rayer.Installer.Abstractions;
using System.IO;
using System.Reflection;

namespace Rayer.Installer.Services;

internal class ResourceExtractor : IResourceExtractor
{
    private readonly Assembly _assembly;

    public ResourceExtractor()
    {
        _assembly = Assembly.GetExecutingAssembly();
    }

    public Stream? GetResource(string name)
    {
        return _assembly.GetManifestResourceStream(name);
    }
}