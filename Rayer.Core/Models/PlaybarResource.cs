using System.Windows.Media;

namespace Rayer.Core.Models;

public struct PlaybarResource
{
    public ImageSource Previous { get; set; }
    public ImageSource Next { get; set; }
    public ImageSource Play { get; set; }
    public ImageSource Pause { get; set; }
}