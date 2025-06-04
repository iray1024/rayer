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

        var base64Token = reader.ReadToEnd();
        Token = Encoding.UTF8.GetString(Convert.FromBase64String(base64Token));
    }

    public string Token { get; private set; } = string.Empty;
}