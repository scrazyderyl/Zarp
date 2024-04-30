using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Zarp.Common.Util
{
    internal class PInvoke
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public const int OBJID_WINDOW = 0;

        public const int CHILDID_SELF = 0;

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        public const uint EVENT_OBJECT_CREATE = 0x8000;
        public const uint EVENT_OBJECT_DESTROY = 0x8001;
        public const uint EVENT_OBJECT_SHOW = 0x8002;
        public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        public const long WS_EX_TOOLWINDOW = 0x00000080L;

        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, int nSize);

        public static int PROCESS_QUERY_INFORMATION = 0x0400;
        public static int PROCESS_VM_READ = 0x0010;
        public static int MAX_PATH = 260;

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        public const uint WM_CLOSE = 0x0010;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const int SW_RESTORE = 9;
        public const int MINIMIZE = 6;

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

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
    }
}
