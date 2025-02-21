using Windows.Graphics;

namespace WinUI3NotifyIcon.Utilities;

public class NotifyIconMenuOpeningEventArgs(PointInt32 point)
{
    public PointInt32 Point { get; } = point;
}
