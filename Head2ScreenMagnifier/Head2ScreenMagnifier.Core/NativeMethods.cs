using System;
using System.Runtime.InteropServices;

namespace Head2ScreenMagnifier.Core
{
    internal static class NativeMethods
    {
        internal static IntPtr HWND_TOPMOST = new IntPtr(-1);

        internal const int USER_TIMER_MINIMUM = 0x0000000A;
        internal const int SM_ARRANGE = 0x38;
        internal const int SM_CLEANBOOT = 0x43;
        internal const int SM_CMONITORS = 80;
        internal const int SM_CMOUSEBUTTONS = 0x2b;
        internal const int SM_CXBORDER = 5;
        internal const int SM_CXCURSOR = 13;
        internal const int SM_CXDOUBLECLK = 0x24;
        internal const int SM_CXDRAG = 0x44;
        internal const int SM_CXEDGE = 0x2d;
        internal const int SM_CXFIXEDFRAME = 7;
        internal const int SM_CXFOCUSBORDER = 0x53;
        internal const int SM_CXFRAME = 0x20;
        internal const int SM_CXHSCROLL = 0x15;
        internal const int SM_CXHTHUMB = 10;
        internal const int SM_CXICON = 11;
        internal const int SM_CXICONSPACING = 0x26;
        internal const int SM_CXMAXIMIZED = 0x3d;
        internal const int SM_CXMAXTRACK = 0x3b;
        internal const int SM_CXMENUCHECK = 0x47;
        internal const int SM_CXMENUSIZE = 0x36;
        internal const int SM_CXMIN = 0x1c;
        internal const int SM_CXMINIMIZED = 0x39;
        internal const int SM_CXMINSPACING = 0x2f;
        internal const int SM_CXMINTRACK = 0x22;
        internal const int SM_CXSCREEN = 0;
        internal const int SM_CXSIZE = 30;
        internal const int SM_CXSIZEFRAME = 0x20;
        internal const int SM_CXSMICON = 0x31;
        internal const int SM_CXSMSIZE = 0x34;
        internal const int SM_CXVIRTUALSCREEN = 0x4e;
        internal const int SM_CXVSCROLL = 2;
        internal const int SM_CYBORDER = 6;
        internal const int SM_CYCAPTION = 4;
        internal const int SM_CYCURSOR = 14;
        internal const int SM_CYDOUBLECLK = 0x25;
        internal const int SM_CYDRAG = 0x45;
        internal const int SM_CYEDGE = 0x2e;
        internal const int SM_CYFIXEDFRAME = 8;
        internal const int SM_CYFOCUSBORDER = 0x54;
        internal const int SM_CYFRAME = 0x21;
        internal const int SM_CYHSCROLL = 3;
        internal const int SM_CYICON = 12;
        internal const int SM_CYICONSPACING = 0x27;
        internal const int SM_CYKANJIWINDOW = 0x12;
        internal const int SM_CYMAXIMIZED = 0x3e;
        internal const int SM_CYMAXTRACK = 60;
        internal const int SM_CYMENU = 15;
        internal const int SM_CYMENUCHECK = 0x48;
        internal const int SM_CYMENUSIZE = 0x37;
        internal const int SM_CYMIN = 0x1d;
        internal const int SM_CYMINIMIZED = 0x3a;
        internal const int SM_CYMINSPACING = 0x30;
        internal const int SM_CYMINTRACK = 0x23;
        internal const int SM_CYSCREEN = 1;
        internal const int SM_CYSIZE = 0x1f;
        internal const int SM_CYSIZEFRAME = 0x21;
        internal const int SM_CYSMCAPTION = 0x33;
        internal const int SM_CYSMICON = 50;
        internal const int SM_CYSMSIZE = 0x35;
        internal const int SM_CYVIRTUALSCREEN = 0x4f;
        internal const int SM_CYVSCROLL = 20;
        internal const int SM_CYVTHUMB = 9;
        internal const int SM_DBCSENABLED = 0x2a;
        internal const int SM_DEBUG = 0x16;
        internal const int SM_MENUDROPALIGNMENT = 40;
        internal const int SM_MIDEASTENABLED = 0x4a;
        internal const int SM_MOUSEPRESENT = 0x13;
        internal const int SM_MOUSEWHEELPRESENT = 0x4b;
        internal const int SM_NETWORK = 0x3f;
        internal const int SM_PENWINDOWS = 0x29;
        internal const int SM_REMOTESESSION = 0x1000;
        internal const int SM_SAMEDISPLAYFORMAT = 0x51;
        internal const int SM_SECURE = 0x2c;
        internal const int SM_SHOWSOUNDS = 70;
        internal const int SM_SWAPBUTTON = 0x17;
        internal const int SM_XVIRTUALSCREEN = 0x4c;
        internal const int SM_YVIRTUALSCREEN = 0x4d;

        internal const string WC_MAGNIFIER = "Magnifier";

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        internal static extern IntPtr CreateWindowEx(int dwExStyle, UInt16 regResult, string lpWindowName, UInt32 dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);


        [DllImport("user32.dll")]
        internal static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassEx")]
        internal static extern System.UInt16 RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        internal static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr SetTimer(IntPtr hWnd, int nIDEvent, int uElapse, IntPtr lpTimerFunc);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool KillTimer(IntPtr hwnd, int idEvent);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal extern static IntPtr CreateWindow(int dwExStyle, string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, LayeredWindowAttributeFlags dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string modName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref POINT pt);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ClipCursor(RECT pRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InvalidateRect(IntPtr hWnd, IntPtr rect, [MarshalAs(UnmanagedType.Bool)] bool erase);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagInitialize();

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagUninitialize();

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagSetWindowSource(IntPtr hwnd, RECT rect);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagGetWindowSource(IntPtr hwnd, ref RECT pRect);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagSetWindowTransform(IntPtr hwnd, ref Transformation pTransform);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagGetWindowTransform(IntPtr hwnd, ref Transformation pTransform);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagSetWindowFilterList(IntPtr hwnd, int dwFilterMode, int count, IntPtr pHWND);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int MagGetWindowFilterList(IntPtr hwnd, IntPtr pdwFilterMode, int count, IntPtr pHWND);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagSetColorEffect(IntPtr hwnd, ref ColorEffect pEffect);

        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MagGetColorEffect(IntPtr hwnd, ref ColorEffect pEffect);
    }
}