using System.Windows.Media;

namespace Rayer.Core.Models;

public class Audio
{
    public int Id { get; set; }

    public string[] Artists { get; set; } = [];

    public string Title { get; set; } = string.Empty;

    public string Album { get; set; } = string.Empty;

    public TimeSpan Duration { get; set; }

    public ImageSource? Cover { get; set; }

    public string Path { get; set; } = string.Empty;
}