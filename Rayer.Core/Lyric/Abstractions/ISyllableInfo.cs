namespace Rayer.Core.Lyric.Abstractions;

public interface ISyllableInfo
{
    public string Text { get; }

    public int StartTime { get; set; }

    public int EndTime { get; set; }

    public int Duration => EndTime - StartTime;
}