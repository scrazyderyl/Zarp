using System;
using static Zarp.Common.PInvoke.User32;
using static Zarp.Common.Util.Window;

namespace Zarp.Core.Datatypes
{
    internal class WindowLocation
    {
        public RECT WindowRect;
        public bool IsMinimized;
        public bool IsMaximized;

        public WindowLocation(IntPtr hWnd)
        {
            WindowRect = GetWindowRect(hWnd);
            IsMinimized = IsIconic(hWnd);
            IsMaximized = IsZoomed(hWnd);
        }

        public WindowLocation(RECT windowRect, bool isMinimized, bool isMaximized)
        {
            WindowRect = windowRect;
            IsMinimized = isMinimized;
            IsMaximized = isMaximized;
        }
    }
}
