using System;
using System.Threading;

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

        private int lastX = -1;
        private int lastY = -1;

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
            get { return this.magnifierZoomFactor; }
            set
            {
                if (this.magnifierZoomFactor != value)
                {
                    this.magnifierZoomFactor = value;

                    // set the magnification factor
                    Transformation matrix = new Transformation(this.magnifierZoomFactor / this.screenZoomFactor);
                    NativeMethods.MagSetWindowTransform(hwndMag, ref matrix);
                }
            }
        }

        #endregion

        #region Public Methods

        public void UpdateMagnifier(int posX, int posY)
        {
            if ((!initialized) || (this.hwndMag == IntPtr.Zero))
            {
                return;
            }

            int width = (int)((this.magWindowRect.right - this.magWindowRect.left) / this.magnifierZoomFactor);
            int height = (int)((this.magWindowRect.bottom - this.magWindowRect.top) / this.magnifierZoomFactor);

            int widthHost = (this.hostWindowRect.right - this.hostWindowRect.left);
            int heightHost = (this.hostWindowRect.bottom - this.hostWindowRect.top);

            // position must be inside certain border
            if (posX < (width / 2))
            {
                posX = (width / 2);
            }

            if (posX > this.xScreen - (width / 2))
            {
                posX = this.xScreen - (width / 2);
            }

            if (posY > this.yScreen - (height / 2))
            {
                posY = this.yScreen - (height / 2);
            }

            RECT sourceRect = new RECT();
            sourceRect.left = posX - (width / 2);
            sourceRect.top = posY - (height / 2);

            //// don't scroll outside desktop area.
            //if (sourceRect.left < 0)
            //{
            //    sourceRect.left = 0;
            //}

            //if (sourceRect.left > this.xScreen - width)
            //{
            //    sourceRect.left = this.xScreen - width;
            //}

            sourceRect.right = sourceRect.left + width;

            //if (sourceRect.top < 0)
            //{
            //    sourceRect.top = 0;
            //}

            //if (sourceRect.top > this.yScreen - height)
            //{
            //    sourceRect.top = this.yScreen - height;
            //}

            sourceRect.bottom = sourceRect.top + height;

            RECT sourceRectCorrected;
            sourceRectCorrected.left = (int)(sourceRect.left * this.screenZoomFactor);
            sourceRectCorrected.right = (int)(sourceRect.right * this.screenZoomFactor);
            sourceRectCorrected.top = (int)(sourceRect.top * this.screenZoomFactor);
            sourceRectCorrected.bottom = (int)(sourceRect.bottom * this.screenZoomFactor);

            // set the source rectangle for the magnifier control
            NativeMethods.MagSetWindowSource(this.hwndMag, sourceRectCorrected);

            // reclaim topmost status, to prevent unmagnified menus from remaining in view 
            int hostX = posX - (widthHost / 2);
            int hostY = posY - (heightHost / 2);

            if (hostX < 0) hostX = 0;
            if (hostY < 0) hostY = 0;
            if (hostX > this.xScreen - widthHost) hostX = this.xScreen - widthHost;
            if (hostY > this.yScreen - heightHost) hostY = this.yScreen - heightHost;

            NativeMethods.SetWindowPos(this.hwndHost, NativeMethods.HWND_TOPMOST, hostX, hostY, 0, 0, (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOSIZE);

            // make the window opaque
            NativeMethods.SetLayeredWindowAttributes(this.hwndHost, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA);

            // set cursor if outside magnifier
            POINT cursorPos = new POINT();
            NativeMethods.GetCursorPos(ref cursorPos);

            int cursorX = cursorPos.x;
            int cursorY = cursorPos.y;

            if (cursorPos.x < sourceRect.left)
            {
                cursorX = sourceRect.left;
            }
            else if (cursorPos.x > sourceRect.right)
            {
                cursorX = sourceRect.right - 5;
            }

            if (cursorPos.y > sourceRect.bottom)
            {
                cursorY = sourceRect.bottom + 2;
            }
            else if (cursorPos.y < sourceRect.top)
            {
                cursorY = sourceRect.top;
            }

            if (cursorPos.x != cursorX || cursorPos.y != cursorY)
            {
                //NativeMethods.SetCursorPos(cursorX, cursorY);
                bool result = NativeMethods.SetCursorPos(sourceRectCorrected.left + (int)((width * this.screenZoomFactor) / 2), sourceRectCorrected.top + (int)((height * this.screenZoomFactor) / 2));
            }

            // force redraw
            NativeMethods.InvalidateRect(this.hwndMag, IntPtr.Zero, true);
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
            this.hwndHost = custom.CreateNativeWindow(this.hostWindowRect.right, this.hostWindowRect.bottom);

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
            Transformation matrix = new Transformation(this.magnifierZoomFactor / this.screenZoomFactor);
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