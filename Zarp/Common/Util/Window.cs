using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using static Zarp.Common.Util.PInvoke;

namespace Zarp.Common.Util
{
    internal class Window
    {
        public static string? GetWindowTitle(IntPtr hWnd)
        {
            StringBuilder text = new StringBuilder(256);
            GetWindowText(hWnd, text, 256);

            if (Marshal.GetLastWin32Error() == 0)
            {
                return text.ToString();
            }

            return null;
        }

        public static Rectangle? GetWindowRect(IntPtr hWnd)
        {
            if (!PInvoke.GetWindowRect(hWnd, out RECT windowRect))
            {
                return null;
            }

            int left = windowRect.Left + 8;
            int top = windowRect.Top;
            int width = windowRect.Right - windowRect.Left - 16;
            int height = windowRect.Bottom - windowRect.Top - 8;

            if (width < 0 || height < 0)
            {
                return null;
            }

            return new Rectangle(left, top, width, height);
        }

        public static string? GetWindowExecutablePath(IntPtr hWnd)
        {
            if (GetWindowThreadProcessId(hWnd, out uint processId) == 0)
            {
                return null;
            }

            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processId);

            if (hProcess == IntPtr.Zero)
            {
                return null;
            }

            // MAX_PATH if LongPathsEnabled is set to false
            StringBuilder filename = new StringBuilder(MAX_PATH);

            if (GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, MAX_PATH) == 0)
            {
                return null;
            }

            return filename.ToString();
        }

        public static bool MinimizeWindow(IntPtr hWnd)
        {
            return ShowWindowAsync(hWnd, MINIMIZE);
        }

        public static bool RestoreWindow(IntPtr hWnd)
        {
            return ShowWindowAsync(hWnd, SW_RESTORE);
        }

        public static void CloseWindow(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
