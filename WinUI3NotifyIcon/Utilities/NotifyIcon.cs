using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using static WinUI3NotifyIcon.Utilities.Interop;

namespace WinUI3NotifyIcon.Utilities;

public partial class NotifyIcon : IDisposable
{
    protected static WNDPROC DefWindowProc { get; } = Marshal.GetDelegateForFunctionPointer<WNDPROC>(GetProcAddress(GetModuleHandleW("user32.dll"), "DefWindowProcW"));

    private NotifyIconNativeWindow? _window;
    private string _text = string.Empty;
    private bool _added;
    private bool _visible;
    private nint _iconHandle;

    public event EventHandler<NotifyIconMenuOpeningEventArgs>? MenuOpening;

    public NotifyIcon()
    {
        TaskbarCreatedMessage = RegisterWindowMessageW("TaskbarCreated");
        TrayMouseMessage = WM_USER + 1024;
        _window = new NotifyIconNativeWindow(this);
        UpdateIcon(_visible);
    }

    protected virtual uint TrayMouseMessage { get; }
    protected virtual uint TaskbarCreatedMessage { get; }
    public nint WindowHandle => _window?.Handle ?? 0;

    public virtual bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value)
                return;

            UpdateIcon(value);
            _visible = value;
        }
    }

    public virtual string Text
    {
        get => _text;
        set
        {
            if (value == _text)
                return;

            value ??= string.Empty;
            if (value.Length > 63)
                throw new ArgumentOutOfRangeException(nameof(value));

            _text = value;
            if (_added)
            {
                UpdateIcon(true);
            }
        }
    }

    public virtual nint IconHandle
    {
        get => _iconHandle;
        set
        {
            if (_iconHandle == value)
                return;

            _iconHandle = value;
            if (_added)
            {
                UpdateIcon(true);
            }
        }
    }

    public void SetFocusToNotificationArea() => SetFocusToNotificationArea((_window?.Handle).GetValueOrDefault());
    public unsafe static void SetFocusToNotificationArea(nint handle)
    {
        var data = new NOTIFYICONDATAW { cbSize = (uint)sizeof(NOTIFYICONDATAW), hWnd = handle };
        Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_SETFOCUS, data);
    }

    public static void CloseNotificationArea()
    {
        // note when waiting a bit the notification window goes away automatically
        // but this is a hack of some sort... I have not found a way to do it better
        //var handle = FindWindowW("NotifyIconOverflowWindow", null);
        var handle = FindWindowW("NotifyIconOverflowWindow", null);
        if (handle != 0)
        {
            SendMessageW(handle, WM_CLOSE, 0, 0);
        }
        else
        {
            handle = FindWindowW("TopLevelWindowForOverflowXamlIsland", null);
            if (handle != 0)
            {
                ShowWindow(handle, SW_HIDE);
            }
        }
    }

    private unsafe void UpdateIcon(bool showIconInTray)
    {
        var window = _window;
        if (window == null)
            return;

        var data = new NOTIFYICONDATAW
        {
            cbSize = (uint)sizeof(NOTIFYICONDATAW),
            uCallbackMessage = TrayMouseMessage,
            uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE | NOTIFY_ICON_DATA_FLAGS.NIF_TIP | NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP,
            Anonymous = new NOTIFYICONDATAW.AnonymousUnion { uVersion = NOTIFYICON_VERSION_4 },
            hWnd = window.Handle
        };

        if (IconHandle != 0)
        {
            data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_ICON;
            data.hIcon = IconHandle;
        }

        data.szTip = Text;

        if (showIconInTray && IconHandle != 0)
        {
            if (!_added)
            {
                if (!Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_ADD, data))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, data);
                _added = true;
            }
            else
            {
                Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_MODIFY, data);
            }
        }
        else if (_added)
        {
            Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_DELETE, data);
            _added = false;
        }
    }

    protected virtual void OnMenuOpening(object? sender, NotifyIconMenuOpeningEventArgs e) => MenuOpening?.Invoke(sender, e);

    // if needed, override to handle specific events
    protected virtual nint WindowProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        var x = (short)((int)(long)wParam & 0xffff);
        var y = (short)(((int)(long)wParam >> 0x10) & 0xffff);

        if (msg == TrayMouseMessage)
        {
            var tmsg = (uint)lParam.ToInt64();

            switch (tmsg)
            {

                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                    CloseNotificationArea();
                    OnMenuOpening(this, new NotifyIconMenuOpeningEventArgs(new Windows.Graphics.PointInt32(x, y)));
                    break;
            }
        }
        else if (msg == TaskbarCreatedMessage)
        {
            _added = false;
            UpdateIcon(_visible);
        }
        else
        {
            switch (msg)
            {
                case WM_ACTIVATEAPP:
                    return 0;

                case WM_DESTROY:
                    UpdateIcon(false);
                    break;
            }
        }
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private sealed partial class NotifyIconNativeWindow : IDisposable
    {
        private static readonly WNDPROC _windowProc = WindowProc;
        private static readonly ConcurrentDictionary<IntPtr, NotifyIconNativeWindow> _windows = new();
        private static readonly ConcurrentDictionary<IntPtr, NotifyIconNativeWindow> _windowsBeingCreated = new();
        private static long _createIndex;

        private readonly NotifyIcon _notifyIcon;
        private nint _handle;

        public unsafe NotifyIconNativeWindow(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;

            fixed (char* clsName = GetType().FullName)
            {
                var wc = new WNDCLASSW
                {
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_windowProc),
                    lpszClassName = (nint)clsName
                };

                var index = (nint)Interlocked.Increment(ref _createIndex);
                _windowsBeingCreated[index] = this;

                if (RegisterClassW(wc) == 0)
                {
                    // we always register the same class name, so "already exists" is expected
                    var gle = Marshal.GetLastWin32Error();
                    if (gle != ERROR_CLASS_ALREADY_EXISTS)
                        throw new Win32Exception(gle);
                }

                _handle = CreateWindowExW(WS_EX_NOACTIVATE, GetType().FullName!, nameof(NotifyIconNativeWindow), 0, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, 0, 0, GetModuleHandleW(null), index);
                if (_handle == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public nint Handle => _handle;

        public void Dispose()
        {
            var handle = Interlocked.Exchange(ref _handle, 0);
            if (handle != 0)
            {
                SendMessageW(handle, WM_CLOSE, 0, 0);
            }
        }

        private static nint WindowProc(nint hWnd, uint msg, nint wParam, nint lParam)
        {
            if (msg == WM_CREATE && lParam != 0)
            {
                var userData = Marshal.ReadIntPtr(lParam);
                if (_windowsBeingCreated.TryRemove(userData, out var win))
                {
                    _windows[hWnd] = win;
                }
            }

            _windows.TryGetValue(hWnd, out var nativeWindow);
            var notifyIcon = nativeWindow?._notifyIcon;
            if (notifyIcon != null)
            {
                if (msg == WM_NCDESTROY)
                {
                    // this is the very last message the window can receive, remove it from the list
                    _windows.TryRemove(hWnd, out _);
                }
                return notifyIcon.WindowProc(hWnd, msg, wParam, lParam);
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            UpdateIcon(false);
            Interlocked.Exchange(ref _window, null)?.Dispose();
            _iconHandle = 0;
        }
    }

    ~NotifyIcon() { Dispose(disposing: false); }
    public void Dispose() { Dispose(disposing: true); GC.SuppressFinalize(this); }
}
