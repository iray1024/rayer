using Rayer.Core.Lyric.Abstractions;

namespace Rayer.Core.Lyric.Impl.AdditionalFileInfo;

public class GeneralAdditionalInfo : IAdditionalFileInfo
{
    public List<KeyValuePair<string, string>>? Attributes { get; set; }
}