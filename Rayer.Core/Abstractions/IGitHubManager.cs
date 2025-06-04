using Rayer.Core.Framework.Injection;
using System.IO;

namespace Rayer.Core.Abstractions;

public interface IGitHubManager
{
    public string Token { get; }

    internal string EncryptToken(string token);
}

[Inject<IGitHubManager>]
internal sealed class GitHubManagerImpl : IGitHubManager
{
    public GitHubManagerImpl()
    {
        var resourceStream = typeof(GitHubManagerImpl).Assembly.GetManifestResourceStream("Rayer.Core.secrets");
        using var reader = new StreamReader(resourceStream!);

        var secret = reader.ReadToEnd();

        Token = Encoding.UTF8.GetString(Convert.FromBase64String(DecryptToken(secret)));
    }

    public string Token { get; private set; } = string.Empty;

    public string EncryptToken(string token)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < token.Length; i += 16)
        {
            var remainLength = Math.Min(16, token.Length - i);
            var chunk = token.Substring(i, remainLength);

            var chunkArray = chunk.ToCharArray();
            Array.Reverse(chunkArray);
            var reversedChunk = new string(chunkArray);

            sb.Append(reversedChunk);
        }

        return sb.ToString();
    }

    private string DecryptToken(string secret)
    {
        return EncryptToken(secret);
    }
}