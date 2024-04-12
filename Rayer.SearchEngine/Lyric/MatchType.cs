namespace Rayer.SearchEngine.Lyric;

public enum MatchType
{
    Perfect = 100,
    VeryHigh = 99,
    High = 95,
    PrettyHigh = 90,
    Medium = 70,
    Low = 30,
    VeryLow = 10,
    NoMatch = -1,
}