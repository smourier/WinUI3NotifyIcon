# WinUI3NotifyIcon
A WinUI3 application that has a notify icon in the system tray. Once the icon in the notification has been clicked, a full WinUI3's `Window` is displayed.

For example, this consider this window's XAML:

```
<Window
    x:Class="WinUI3NotifyIcon.NotifyIconWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    Title="NotifyIconWindow">

    <StackPanel HorizontalAlignment="Center" Orientation="Vertical">
        <AppBarButton
            Click="Open_Click"
            Icon="OpenPane"
            Label="Open App" />
        <AppBarSeparator />
        <AppBarButton
            Click="Quit_Click"
            Icon="ClosePane"
            Label="Quit" />
    </StackPanel>

</Window>
```

The app will show an icon in the notification area, like this:

![image](https://github.com/user-attachments/assets/45dbeaff-89d7-4fef-ba63-4651cba232d9)

which will, once clicked, display the window:

![image](https://github.com/user-attachments/assets/8802893a-d881-4711-a3c9-9eed757c8121)
