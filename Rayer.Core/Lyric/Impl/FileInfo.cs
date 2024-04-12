using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Enums;

namespace Rayer.Core.Lyric.Impl;

public class FileInfo
{
    public LyricType Type { get; set; }

    public SyncType SyncTypes { get; set; }

    public IAdditionalFileInfo? AdditionalInfo { get; set; }
}