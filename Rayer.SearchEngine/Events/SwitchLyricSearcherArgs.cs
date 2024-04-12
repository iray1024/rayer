namespace Rayer.SearchEngine.Events;

public class SwitchLyricSearcherArgs(bool hasData = true) : EventArgs
{
    public static readonly SwitchLyricSearcherArgs True = new(true);
    public static readonly SwitchLyricSearcherArgs False = new(false);

    public bool HasData { get; set; } = hasData;
}