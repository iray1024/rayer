using System.Windows.Media;

namespace Rayer.Core.Models;

public class Audio
{
    public long Id { get; set; }

    public string[] Artists { get; set; } = [];

    public string Title { get; set; } = string.Empty;

    public string Album { get; set; } = string.Empty;

    public TimeSpan Duration { get; set; }

    public ImageSource? Cover { get; set; }

    public string Path { get; set; } = string.Empty;

    // 后续如果需要同时加入所有搜索项时（Web音源需要进一步调用API才能获取真实播放地址），实现该属性功能
    public bool IsVirualWebSource { get; set; } = false;
}

public class AudioSortComparer : IComparer<Audio>
{
    private readonly bool _ascending;

    public static AudioSortComparer Ascending { get; } = new AudioSortComparer();

    public static AudioSortComparer Descending { get; } = new AudioSortComparer(false);

    private AudioSortComparer(bool ascending = true)
    {
        _ascending = ascending;
    }

    public int Compare(Audio? x, Audio? y)
    {
        return x is null || y is null
            ? 0
            : _ascending
                ? string.Compare(y.Title, x.Title)
                : string.Compare(x.Title, y.Title);
    }
}