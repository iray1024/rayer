namespace Rayer.SearchEngine.Lyric.Providers;

public abstract class Provider : IProvider
{
    public abstract string Name { get; }

    public abstract string DisplayName { get; }

    public abstract IProviderResult ObtainLyrics();
}