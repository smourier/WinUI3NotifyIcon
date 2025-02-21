using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace WinUI3NotifyIcon.Utilities;

public static partial class Interop
{
    public delegate nint WNDPROC(nint hwnd, uint msg, nint wParam, nint lParam);

    [PreserveSig, LibraryImport("KERNEL32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial nint GetModuleHandleW(string? lpModuleName);

    [PreserveSig, LibraryImport("KERNEL32", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    public static partial nint GetProcAddress(nint hModule, string lpProcName);

    [PreserveSig, LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial uint RegisterWindowMessageW(string lpString);

    [PreserveSig, LibraryImport("SHELL32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static partial bool Shell_NotifyIconW(NOTIFY_ICON_MESSAGE dwMessage, in NOTIFYICONDATAW lpData);

    [PreserveSig, LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial nint FindWindowW(string? lpClassName, string? lpWindowName);

    [PreserveSig, LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial nint SendMessageW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [PreserveSig, LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial nint CreateWindowExW(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int X, int Y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

    [PreserveSig, LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial ushort RegisterClassW(in WNDCLASSW lpWndClass);

    [PreserveSig, LibraryImport("USER32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    // trick: RectInt32 is not the same as Win32's RECT so this is private and we provide a public function that does the trick
    private static partial bool CalculatePopupWindowPosition(in PointInt32 anchorPoint, in SizeInt32 windowSize, uint flags, nint excludeRect, out RectInt32 popupWindowPosition);

    [PreserveSig, LibraryImport("USER32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static partial bool SetForegroundWindow(nint hWnd);

    [PreserveSig, LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    public static partial nint LoadImageW(nint hInst, uint name, uint type, int cx, int cy, uint fuLoad);

    [PreserveSig, LibraryImport("USER32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static partial bool DestroyIcon(nint hIcon);

    [PreserveSig, LibraryImport("USER32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static partial bool ShowWindow(nint hWnd, uint nCmdShow);

    public const uint WM_CREATE = 0x0001;
    public const uint WM_DESTROY = 0x0002;
    public const uint WM_CLOSE = 0x0010;
    public const uint WM_ACTIVATEAPP = 0x001C;
    public const uint WM_NCDESTROY = 0x0082;
    public const uint WM_LBUTTONUP = 0x0202;
    public const uint WM_RBUTTONUP = 0x0205;
    public const uint WM_USER = 0x0400;

    public const uint NOTIFYICON_VERSION_4 = 4;
    public const int CW_USEDEFAULT = unchecked((int)0x80000000);
    public const uint WS_EX_NOACTIVATE = 0x08000000;
    public const int ERROR_CLASS_ALREADY_EXISTS = 1410;
    public const uint IMAGE_ICON = 1;
    public const uint IDI_APPLICATION = 0x00007F00;
    public const uint TPM_WORKAREA = 0x00010000;
    public const uint SW_HIDE = 0;

    public enum NOTIFY_ICON_MESSAGE : uint
    {
        NIM_ADD = 0,
        NIM_MODIFY = 1,
        NIM_DELETE = 2,
        NIM_SETFOCUS = 3,
        NIM_SETVERSION = 4,
    }

    [Flags]
    public enum NOTIFY_ICON_DATA_FLAGS : uint
    {
        NIF_MESSAGE = 1,
        NIF_ICON = 2,
        NIF_TIP = 4,
        NIF_STATE = 8,
        NIF_INFO = 16,
        NIF_GUID = 32,
        NIF_REALTIME = 64,
        NIF_SHOWTIP = 128,
    }

    [Flags]
    public enum NOTIFY_ICON_STATE : uint
    {
        NIS_HIDDEN = 1,
        NIS_SHAREDICON = 2,
    }

    [Flags]
    public enum NOTIFY_ICON_INFOTIP_FLAGS : uint
    {
        NIIF_NONE = 0,
        NIIF_INFO = 1,
        NIIF_WARNING = 2,
        NIIF_ERROR = 3,
        NIIF_USER = 4,
        NIIF_ICON_MASK = 15,
        NIIF_NOSOUND = 16,
        NIIF_LARGE_ICON = 32,
        NIIF_RESPECT_QUIET_TIME = 128,
    }

    public partial struct WNDCLASSW
    {
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public nint lpszMenuName;
        public nint lpszClassName;
    }

    public partial struct NOTIFYICONDATAW
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct AnonymousUnion
        {
            [FieldOffset(0)]
            public uint uTimeout;

            [FieldOffset(0)]
            public uint uVersion;
        }

        public uint cbSize;
        public nint hWnd;
        public uint uID;
        public NOTIFY_ICON_DATA_FLAGS uFlags;
        public uint uCallbackMessage;
        public nint hIcon;
        public InlineArraySystemChar_128 szTip;
        public NOTIFY_ICON_STATE dwState;
        public NOTIFY_ICON_STATE dwStateMask;
        public InlineArraySystemChar_256 szInfo;
        public AnonymousUnion Anonymous;
        public InlineArraySystemChar_64 szInfoTitle;
        public NOTIFY_ICON_INFOTIP_FLAGS dwInfoFlags;
        public Guid guidItem;
        public nint hBalloonIcon;
    }

    [InlineArray(Length)]
    public partial struct InlineArraySystemChar_64
    {
        public const int Length = 64;
        public char Data;

        public override readonly string ToString() => ((ReadOnlySpan<char>)this).ToString().TrimEnd('\0');
        public void CopyFrom(string? str) => CopyFrom<InlineArraySystemChar_64>(str, this, Length);
        public static implicit operator InlineArraySystemChar_64(string? str) { var n = new InlineArraySystemChar_64(); n.CopyFrom(str); return n; }
    }

    [InlineArray(Length)]
    public partial struct InlineArraySystemChar_256
    {
        public const int Length = 256;
        public char Data;

        public override readonly string ToString() => ((ReadOnlySpan<char>)this).ToString().TrimEnd('\0');
        public void CopyFrom(string? str) => CopyFrom<InlineArraySystemChar_256>(str, this, Length);
        public static implicit operator InlineArraySystemChar_256(string? str) { var n = new InlineArraySystemChar_256(); n.CopyFrom(str); return n; }
    }

    [InlineArray(Length)]
    public partial struct InlineArraySystemChar_128
    {
        public const int Length = 128;
        public char Data;

        public override readonly string ToString() => ((ReadOnlySpan<char>)this).ToString().TrimEnd('\0');
        public void CopyFrom(string? str) => CopyFrom<InlineArraySystemChar_128>(str, this, Length);
        public static implicit operator InlineArraySystemChar_128(string? str) { var n = new InlineArraySystemChar_128(); n.CopyFrom(str); return n; }
    }

    private static void CopyFrom<T>(string? str, Span<char> chars, int length)
    {
        for (var i = 0; i < length; i++)
        {
            if (str != null && i < str.Length)
            {
                chars[i] = str[i];
            }
            else
            {
                chars[i] = '\0';
            }
        }
    }

    public static RectInt32 CalculatePopupWindowPosition(PointInt32 anchorPoint, SizeInt32 windowSize, uint flags, nint excludeRect)
    {
        CalculatePopupWindowPosition(anchorPoint, windowSize, flags, excludeRect, out var rc);
        return new RectInt32(rc.X, rc.Y, rc.Width - rc.X, rc.Height - rc.Y);
    }

    public static nint GetApplicationIconHandle(int size = 16)
    {
        var path = Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName);
        if (path == null)
            return 0;

        var exeHandle = GetModuleHandleW(path);
        return LoadImageW(exeHandle, IDI_APPLICATION, IMAGE_ICON, size, size, 0);
    }
}
