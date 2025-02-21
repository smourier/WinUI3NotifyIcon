using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace WinUI3NotifyIcon;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(App.IconHandle));
    }

    private void MyButton_Click(object sender, RoutedEventArgs e)
    {
        myButton.Content = "Clicked";
    }
}
