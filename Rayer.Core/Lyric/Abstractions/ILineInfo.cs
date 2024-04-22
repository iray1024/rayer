using Rayer.Core.Lyric.Enums;
using Rayer.Core.Lyric.Utils;

namespace Rayer.Core.Lyric.Abstractions;

public interface ILineInfo : IComparable
{
    public string Text { get; }

    public int? StartTime { get; set; }

    public int? EndTime { get; set; }

    public int? Duration => EndTime - StartTime;

    public int? StartTimeWithSubLine => MathUtils.Min(StartTime, SubLine?.StartTime);

    public int? EndTimeWithSubLine => MathUtils.Max(EndTime, SubLine?.EndTime);

    public int? DurationWithSubLine => EndTimeWithSubLine - StartTimeWithSubLine;

    public LyricAlignment LyricsAlignment { get; }

    public ILineInfo? SubLine { get; }

    public string FullText
    {
        get
        {
            if (SubLine == null)
            {
                return Text;
            }
            else
            {
                var sb = new StringBuilder();
                if (SubLine.StartTime < StartTime)
                {
                    sb.Append('(');
                    sb.Append(RemoveFrontBackBrackets(SubLine.Text));
                    sb.Append(") ");
                    sb.Append(Text.Trim());
                }
                else
                {
                    sb.Append(Text.Trim());
                    sb.Append(" (");
                    sb.Append(RemoveFrontBackBrackets(SubLine.Text));
                    sb.Append(')');
                }
                return sb.ToString();
            }
        }
    }

    private static string RemoveFrontBackBrackets(string source)
    {
        if (source == null)
        {
            return string.Empty;
        }

        source = source.Trim();

        if (source[0] is '(' or '（')
        {
            source = source[1..];
        }

        if (source[^1] is ')' or '）')
        {
            source = source[..^1];
        }

        return source.Trim();
    }
}