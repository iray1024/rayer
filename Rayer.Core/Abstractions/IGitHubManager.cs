using Rayer.Core.Framework.Injection;
using System.IO;

namespace Rayer.Core.Abstractions;

public interface IGitHubManager
{
    public string Token { get; }
}

[Inject<IGitHubManager>]
internal sealed class GitHubManagerImpl : IGitHubManager
{
    public GitHubManagerImpl()
    {
        var resourceStream = typeof(GitHubManagerImpl).Assembly.GetManifestResourceStream("Rayer.Core.secrets");
        using var reader = new StreamReader(resourceStream!);

        Token = reader.ReadToEnd();
    }

    public string Token { get; private set; } = string.Empty;
}