using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUI3NotifyIcon;

public sealed partial class NotifyIconWindow : Window
{
    public NotifyIconWindow()
    {
        InitializeComponent();

        // use app's icon
        AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(App.IconHandle));

        // hide when deactivated
        Activated += (s, e) =>
        {
            if (e.WindowActivationState == WindowActivationState.Deactivated)
            {
                AppWindow.Hide();
            }
        };

        // hide when attempting closing
        AppWindow.Closing += (s, e) =>
        {
            e.Cancel = true;
            s.Hide();
        };

        // don't show in task bars
        AppWindow.IsShownInSwitchers = false;

        // don't show controls
        AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        var presenter = (OverlappedPresenter)AppWindow.Presenter;
        presenter.IsResizable = false;
        presenter.SetBorderAndTitleBar(false, false);
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
    }

    private void Open_Click(object sender, RoutedEventArgs e) => App.MainWindow?.Activate();
    private void Quit_Click(object sender, RoutedEventArgs e) => Application.Current.Exit();
}
