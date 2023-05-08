using System;

namespace Head2ScreenMagnifier.Lib
{
    public class Magnifier : IDisposable
    {
        #region Private Variables

        private IntPtr hwndHost;
        private IntPtr hwndMag;

        private RECT magWindowRect = new RECT();
        private RECT hostWindowRect = new RECT();

        private float magnifierZoomFactor;
        private float screenZoomFactor;
        private float magnifierWindowSizeFactor;

        private bool initialized;

        private int xScreen = 0;
        private int yScreen = 0;

        #endregion

        #region Constructor & Destructor

        public Magnifier(float magnifierZoomFactor, float screenZoomFactor, float magnifierWindowSizeFactor)
        {
            this.magnifierZoomFactor = magnifierZoomFactor;
            this.screenZoomFactor = screenZoomFactor;
            this.magnifierWindowSizeFactor = magnifierWindowSizeFactor;

            this.initialized = NativeMethods.MagInitialize();

            if (this.initialized)
            {
                this.xScreen = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN);
                this.yScreen = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN);

                this.SetupMagnifier();
            }
        }

        ~Magnifier()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public float Magnification
        {
            get { return magnifierZoomFactor; }
            set
            {
                if (magnifierZoomFactor != value)
                {
                    magnifierZoomFactor = value;

                    // set the magnification factor
                    Transformation matrix = new Transformation(magnifierZoomFactor);
                    NativeMethods.MagSetWindowTransform(hwndMag, ref matrix);
                }
            }
        }

        #endregion

        #region Public Methods

        public void UpdateMagnifier(int x, int y)
        {
            if ((!initialized) || (this.hwndMag == IntPtr.Zero))
            {
                return;
            }

            POINT mousePoint = new POINT(x, y);
            RECT sourceRect = new RECT();

            int width = (int)((this.magWindowRect.right - this.magWindowRect.left) / magnifierZoomFactor);
            int height = (int)((this.magWindowRect.bottom - this.magWindowRect.top) / magnifierZoomFactor);

            int widthHost = (this.hostWindowRect.right - this.hostWindowRect.left);
            int heightHost = (this.hostWindowRect.bottom - this.hostWindowRect.top);

            sourceRect.left = mousePoint.x - width / 2;
            sourceRect.top = mousePoint.y - height / 2;

            // don't scroll outside desktop area.
            if (sourceRect.left < 0)
            {
                sourceRect.left = 0;
            }

            if (sourceRect.left > this.xScreen - width)
            {
                sourceRect.left = this.xScreen - width;
            }

            sourceRect.right = sourceRect.left + width;

            if (sourceRect.top < 0)
            {
                sourceRect.top = 0;
            }

            if (sourceRect.top > this.yScreen - height)
            {
                sourceRect.top = this.yScreen - height;
            }

            RECT sourceRectCorrected;
            sourceRectCorrected.left = (int)(sourceRect.left * this.screenZoomFactor);
            sourceRectCorrected.right = (int)(sourceRect.right * this.screenZoomFactor);
            sourceRectCorrected.top = (int)(sourceRect.top * this.screenZoomFactor);
            sourceRectCorrected.bottom = (int)(sourceRect.bottom * this.screenZoomFactor);

            // set the source rectangle for the magnifier control
            NativeMethods.MagSetWindowSource(hwndMag, sourceRectCorrected);

            // reclaim topmost status, to prevent unmagnified menus from remaining in view 
            int hostX = mousePoint.x - (widthHost / 2);
            int hostY = mousePoint.y - (heightHost / 2);

            if (hostX < 0) hostX = 0;
            if (hostY < 0) hostY = 0;
            if (hostX > this.xScreen - widthHost) hostX = this.xScreen - widthHost;
            if (hostY > this.yScreen - heightHost) hostY = this.yScreen - heightHost;

            NativeMethods.SetWindowPos(this.hwndHost, NativeMethods.HWND_TOPMOST, hostX, hostY, 0, 0, (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOSIZE);

            // make the window opaque
            NativeMethods.SetLayeredWindowAttributes(this.hwndHost, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);

            // force redraw
            NativeMethods.InvalidateRect(hwndMag, IntPtr.Zero, true);
        }

        public void UpdateCursorPosition()
        {
            // set cursor position when window stopped moving
            bool result = NativeMethods.SetCursorPos(this.magWindowRect.right + 50, this.magWindowRect.bottom + 100);
        }

        #endregion

        #region Private Helper

        private void SetupMagnifier()
        {
            if (!this.initialized)
            {
                return;
            }

            // set bounds of host window according to screen size
            this.hostWindowRect.top = 0;
            this.hostWindowRect.bottom = (int)(this.yScreen * this.magnifierWindowSizeFactor);
            this.hostWindowRect.left = 0;
            this.hostWindowRect.right = (int)(this.xScreen * this.magnifierWindowSizeFactor);

            NativeWindow custom = new NativeWindow();
            this.hwndHost = custom.CreateNativeWindow(hostWindowRect.right, hostWindowRect.bottom);

            // make the window opaque
            NativeMethods.SetLayeredWindowAttributes(this.hwndHost, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);

            IntPtr hInst = NativeMethods.GetModuleHandle(null);

            // create a magnifier control that fills the client area
            NativeMethods.GetClientRect(this.hwndHost, ref this.magWindowRect);

            this.hwndMag = NativeMethods.CreateWindow((int)ExtendedWindowStyles.WS_EX_CLIENTEDGE, NativeMethods.WC_MAGNIFIER, "MagnifierWindow", (int)WindowStyles.WS_CHILD | (int)MagnifierStyle.MS_SHOWMAGNIFIEDCURSOR | (int)WindowStyles.WS_VISIBLE, this.magWindowRect.left, this.magWindowRect.top, this.magWindowRect.right, this.magWindowRect.bottom, this.hwndHost, IntPtr.Zero, hInst, IntPtr.Zero);

            if (this.hwndMag == IntPtr.Zero)
            {
                return;
            }

            // set the magnification factor
            Transformation matrix = new Transformation(this.magnifierZoomFactor);
            NativeMethods.MagSetWindowTransform(this.hwndMag, ref matrix);
        }

        private void RemoveMagnifier()
        {
            if (this.initialized)
            {
                NativeMethods.MagUninitialize();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.RemoveMagnifier();
        }

        #endregion
    }
}