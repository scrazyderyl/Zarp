﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Zarp.Common.Util
{
    public class PInvoke
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        public const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
        public const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
        public const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        public const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        public const uint EVENT_OBJECT_DESTROY = 0x8001;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        public static IntPtr SubscribeWinEvent(uint eventConstant, WinEventDelegate dele, uint processId = 0)
        {
            return SetWinEventHook(eventConstant, eventConstant, IntPtr.Zero, dele, processId, 0, WINEVENT_OUTOFCONTEXT);
        }

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

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        public static Rectangle? GetWindowRect(IntPtr hWnd)
        {
            RECT windowRect;
            RECT clientRect;

            if (!GetWindowRect(hWnd, out windowRect) || !GetClientRect(hWnd, out clientRect))
            {
                return null;
            }

            int width = clientRect.Right - clientRect.Left;
            int height = clientRect.Bottom - clientRect.Top;
            int left = windowRect.Left + 8;
            int top = windowRect.Bottom - height - 8;

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
            uint processId;

            if (GetWindowThreadProcessId(hWnd, out processId) == 0)
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

        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);

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