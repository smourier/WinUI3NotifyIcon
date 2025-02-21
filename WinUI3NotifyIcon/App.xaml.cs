using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinUI3NotifyIcon.Utilities;

namespace WinUI3NotifyIcon;

public partial class App : Application
{
    // for this project to work, it *must* have something like that in .csproj, otherwise use some other icon
    //   <PropertyGroup>
    //     <ApplicationIcon>WinUI3NotifyIcon.ico</ApplicationIcon>
    //   </PropertyGroup>
    public static nint IconHandle { get; } = Interop.GetApplicationIconHandle();
    public static Window? MainWindow { get; private set; }

    private NotifyIcon? _notifyIcon;
    private NotifyIconWindow? _menu;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Closed += (s, e) =>
        {
            _menu?.Close();
            _notifyIcon?.Dispose();
            if (IconHandle != 0)
            {
                Interop.DestroyIcon(IconHandle);
            }
        };
        MainWindow.Activate();

        _notifyIcon = new NotifyIcon
        {
            Text = "WinUI3 Notify Icon"
        };

        _notifyIcon.IconHandle = IconHandle;
        _notifyIcon.MenuOpening += (s, e) => ShowAsNotifyIcon(e.Point);
        _notifyIcon.Visible = true;
    }

    private void ShowAsNotifyIcon(PointInt32 cursor)
    {
        if (_menu == null)
        {
            _menu = new NotifyIconWindow();
            _menu.AppWindow.Resize(new SizeInt32(100, 150));
        }

        var rc = Interop.CalculatePopupWindowPosition(cursor, _menu.AppWindow.Size, Interop.TPM_WORKAREA, 0);
        _menu.AppWindow.MoveAndResize(rc);
        _menu.AppWindow.Show(true);

        // this is useful if main is minimized
        Interop.SetForegroundWindow(Win32Interop.GetWindowFromWindowId(_menu.AppWindow.Id));
    }
}
