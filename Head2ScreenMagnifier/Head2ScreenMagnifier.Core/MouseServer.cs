using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Head2ScreenMagnifier.Core
{
    public class MouseServer
    {
        public Vector2 GetPosition()
        {
            POINT cursorPos = new POINT();
            NativeMethods.GetCursorPos(ref cursorPos);
            return new System.Numerics.Vector2(cursorPos.x, cursorPos.y);
        }
    }
}
