using System;
using System.Runtime.InteropServices;

namespace Head2ScreenMagnifier.Lib
{
    delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    internal class NativeWindow
    {
        const UInt32 CS_DBLCLKS = 8;
        const UInt32 CS_VREDRAW = 1;
        const UInt32 CS_HREDRAW = 2;
        const UInt32 COLOR_BTNFACE = 15;
        const UInt32 IDC_ARROW = 32512;
        const UInt32 WM_DESTROY = 2;
        const UInt32 WM_PAINT = 0x0f;

        const UInt32 WS_EX_TOPMOST = 0x00000008;
        const UInt32 WS_EX_LAYERED = 0x00080000;
        const UInt32 WS_EX_TRANSPARENT = 0x00000020;
        const UInt32 WS_CLIPCHILDREN = 0x02000000;

        private WndProc delegWndProc = MyWndProc;

        internal IntPtr CreateNativeWindow(int width, int height)
        {
            WNDCLASSEX wind_class = new WNDCLASSEX();
            wind_class.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            wind_class.style = (int)(CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS);
            wind_class.hbrBackground = (IntPtr)(COLOR_BTNFACE + 1); 
            wind_class.cbClsExtra = 0;
            wind_class.cbWndExtra = 0;
            wind_class.hInstance = Marshal.GetHINSTANCE(this.GetType().Module);
            wind_class.hIcon = IntPtr.Zero;
            wind_class.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, (int)IDC_ARROW);
            wind_class.lpszMenuName = null;
            wind_class.lpszClassName = NativeMethods.WC_MAGNIFIER;
            wind_class.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(delegWndProc);
            wind_class.hIconSm = IntPtr.Zero;
            ushort regResult = NativeMethods.RegisterClassEx(ref wind_class);

            if (regResult == 0)
            {
                uint error = NativeMethods.GetLastError();
                return IntPtr.Zero;
            }

            string wndClass = wind_class.lpszClassName;

            IntPtr hWnd = NativeMethods.CreateWindowEx((int)WS_EX_TOPMOST | (int)WS_EX_LAYERED | (int)WS_EX_TRANSPARENT, regResult, "Screen Magnifier", (int)WS_CLIPCHILDREN, 0, 0, width, height, IntPtr.Zero, IntPtr.Zero, wind_class.hInstance, IntPtr.Zero);

            if (hWnd == ((IntPtr)0))
            {
                uint error = NativeMethods.GetLastError();
                return IntPtr.Zero;
            }

            NativeMethods.ShowWindow(hWnd, 1);
            NativeMethods.UpdateWindow(hWnd);

            return hWnd;
        }

        private static IntPtr MyWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                // all GUI painting must be done here
                case WM_PAINT:
                    break;

                case WM_DESTROY:
                    NativeMethods.DestroyWindow(hWnd);

                    // if you want to shutdown the application, call the next function instead of DestroyWindow
                    // PostQuitMessage(0);
                    break;

                default:
                    break;
            }

            return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}