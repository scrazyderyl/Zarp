using System;
using System.Runtime.InteropServices;
using Zarp.Common.PInvoke;
using static Zarp.Common.PInvoke.DwmApi;
using static Zarp.Common.PInvoke.Kernel32;
using static Zarp.Common.PInvoke.Psapi;
using static Zarp.Common.PInvoke.User32;
using static Zarp.Common.PInvoke.Winerror;

namespace Zarp.Common.Util
{
    internal class Window
    {
        public static string? GetWindowTitle(IntPtr hWnd)
        {
            char[] text = new char[256];
            int length = GetWindowText(hWnd, text, 256);

            if (Marshal.GetLastWin32Error() == 0)
            {
                return new string(text, 0, length);
            }

            return null;
        }

        public static string? GetWindowClassName(IntPtr hWnd)
        {
            char[] text = new char[256];
            int length = GetClassName(hWnd, text, 256);

            if (length != 0)
            {
                return new string(text, 0, length);
            }

            return null;
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

            char[] filename = new char[MAX_PATH];
            int length = GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, MAX_PATH);
            CloseHandle(hProcess);

            if (length == 0)
            {
                return null;
            }

            return new string(filename, 0, length);
        }

        public static RECT GetWindowRect(IntPtr hWnd)
        {
            if (DwmGetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, (uint)Marshal.SizeOf(typeof(RECT))) != S_OK)
            {
                throw new Exception();
            }

            return rect;
        }

        public static RECT GetWindowRectWithShadow(IntPtr hWnd)
        {
            if (!User32.GetWindowRect(hWnd, out RECT rect))
            {
                throw new Exception();
            }

            return rect;
        }

        public static bool SetWindowLocation(IntPtr hWnd, RECT location)
        {
            try
            {
                RECT withoutShadow = GetWindowRect(hWnd);
                RECT withShadow = GetWindowRectWithShadow(hWnd);

                int leftShadow = withoutShadow.Left - withShadow.Left;
                int rightShadow = withShadow.Right - withoutShadow.Right;
                int bottomShadow = withShadow.Bottom - withoutShadow.Bottom;

                int x = location.Left - leftShadow;
                int y = location.Top;
                int width = location.Right - location.Left + leftShadow + rightShadow;
                int height = location.Bottom - location.Top + bottomShadow;

                return MoveWindow(hWnd, x, y, width, height, true);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsWindowOwned(IntPtr hWnd) => GetWindow(hWnd, GW_OWNER) != IntPtr.Zero;

        public static bool MinimizeWindow(IntPtr hWnd) => ShowWindowAsync(hWnd, SW_MINIMIZE);

        public static bool MaximizeWindow(IntPtr hWnd) => ShowWindowAsync(hWnd, SW_MAXIMIZE);

        public static bool RestoreWindow(IntPtr hWnd) => ShowWindowAsync(hWnd, SW_RESTORE);

        public static void CloseWindow(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
