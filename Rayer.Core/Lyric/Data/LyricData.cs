using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Impl;

namespace Rayer.Core.Lyric.Data;

public class LyricData
{
    public FileInfo? File { get; set; }

    public List<ILineInfo>? Lines { get; set; }

    public List<string>? Writers { get; set; }

    public ITrackMetadata? TrackMetadata { get; set; }
}