using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Zarp.Common.Util
{
    public class PInvoke
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public const int OBJID_WINDOW = 0;

        public const int CHILDID_SELF = 0;

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        public const uint EVENT_OBJECT_DESTROY = 0x8001;
        public const uint EVENT_OBJECT_SHOW = 0x8002;
        public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        public const int WS_EX_TOOLWINDOW = 0x00000080;

        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

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

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static Rectangle? GetWindowRect(IntPtr hWnd)
        {
            RECT windowRect;

            if (!GetWindowRect(hWnd, out windowRect))
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

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("psapi.dll")]
        public static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, int nSize);

        public static int PROCESS_QUERY_INFORMATION = 0x0400;
        public static int PROCESS_VM_READ = 0x0010;
        public static int MAX_PATH = 260;

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

            StringBuilder filename = new StringBuilder(MAX_PATH);

            if (GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, MAX_PATH) == 0)
            {
                return null;
            }

            return filename.ToString();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        public const uint WM_CLOSE = 0x0010;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void CloseWindow(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        public const int SW_RESTORE = 9;
        public const int MINIMIZE = 6;

        public static bool MinimizeWindow(IntPtr hWnd)
        {
            return ShowWindowAsync(hWnd, MINIMIZE);
        }

        public static bool RestoreWindow(IntPtr hWnd)
        {
            return ShowWindowAsync(hWnd, SW_RESTORE);
        }

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_ASYNCWINDOWPOS = 0x4000;

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct POINT
        {
            public int X;
            public int Y;
        }
    }
}
