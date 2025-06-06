using Rayer.FrameworkCore.Injection;

namespace Rayer.FrameworkCore;

public interface IGitHubManager
{
    public string Token { get; }

    internal string EncryptToken(string token);
}

[Inject<IGitHubManager>]
internal sealed class GitHubManagerImpl : IGitHubManager
{
    private const string EncryptedSecret = "x8FdhB3XiVHa0l2Z4EmMwElTPpVWMFUM1E0XzkkVGZDb61Udl5EbI9WayN1ZJdHeFNUUEpGSwFnTnRDeup1R2MTRnlnRJRzTyMUVBZkVHlzcjhUZ202M21mNwNnS";

    public GitHubManagerImpl()
    {
        Token = Encoding.UTF8.GetString(Convert.FromBase64String(DecryptToken(EncryptedSecret)));
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