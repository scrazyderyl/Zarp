using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zarp.Core.Datatypes;
using static Zarp.Common.PInvoke.User32;
using static Zarp.Common.Util.Window;

namespace Zarp.Core.App
{
    internal class ProcessManager : IDisposable
    {
        public ApplicationInfo? ApplicationInfo
        {
            get => _ApplicationInfo;
        }

        private Process _Process;
        private ApplicationInfo? _ApplicationInfo;
        private Dictionary<string, List<WindowLocation?>> _WindowLayout;
        private Dictionary<IntPtr, WindowState> _Windows;
        private ProcessWinEventListener _WinEventListener;
        private bool _BlockingEnabled;
        private bool _LayoutRestoreEnabled;

        public ProcessManager(Process process) : this(process, new Dictionary<string, List<WindowLocation?>>()) { }

        public ProcessManager(Process process, Dictionary<string, List<WindowLocation?>> layout)
        {
            _Process = process;
            _ApplicationInfo = process.MainModule != null && process.MainModule.FileName != null ? new ApplicationInfo(process.MainModule.FileName) : null;
            _WindowLayout = layout;
            _Windows = new Dictionary<IntPtr, WindowState>();
            _WinEventListener = new ProcessWinEventListener(process.Id);

            _WinEventListener.Subscribe(EVENT_OBJECT_DESTROY, ObjectDestroyed);
            _WinEventListener.Subscribe(EVENT_OBJECT_LOCATIONCHANGE, ObjectLocationChanged);
            _WinEventListener.Enable();
        }

        private static bool IsNoActivateSet(IntPtr hWnd)
        {
            long style = (long)GetWindowLongPtr(hWnd, GWL_EXSTYLE);

            if (style == 0)
            {
                throw new Exception();
            }

            return (style & WS_EX_NOACTIVATE) != 0;
        }

        private static bool SetNoActivate(IntPtr hWnd, bool value)
        {
            long style = (long)GetWindowLongPtr(hWnd, GWL_EXSTYLE);

            if (style == 0)
            {
                return false;
            }

            style = value ? style | WS_EX_NOACTIVATE : style & ~WS_EX_NOACTIVATE;

            // Does not update extended style
            if (SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(style)) == IntPtr.Zero)
            {
                return false;
            }

            return true;
        }

        private static bool MoveWindow(IntPtr hWnd, WindowLocation location)
        {
            if (!SetWindowLocation(hWnd, location.WindowRect))
            {
                return false;
            }

            if (location.IsMaximized && !MaximizeWindow(hWnd))
            {
                return false;
            }

            if (location.IsMinimized && !MinimizeWindow(hWnd))
            {
                return false;
            }

            return true;
        }

        private void ObjectDestroyed(IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (idObject != OBJID_WINDOW)
            {
                return;
            }

            if (_Windows.Remove(hwnd, out WindowState? windowState))
            {
                RemoveWindow(hwnd, windowState);
            }
        }

        private void ObjectLocationChanged(IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (idObject != OBJID_WINDOW)
            {
                return;
            }

            if (_LayoutRestoreEnabled)
            {
                UpdateLocationInfo(hwnd);
            }
        }

        public void EnableBlocking()
        {
            if (_BlockingEnabled == true)
            {
                return;
            }

            foreach (KeyValuePair<IntPtr, WindowState> window in _Windows)
            {
                Block(window.Key, window.Value);
            }

            _BlockingEnabled = true;
        }

        public void DisableBlocking()
        {
            if (_BlockingEnabled == false)
            {
                return;
            }

            foreach (KeyValuePair<IntPtr, WindowState> window in _Windows)
            {
                Unblock(window.Key, window.Value);
            }

            _BlockingEnabled = false;
        }

        public void EnableLayoutRestore()
        {
            if (_LayoutRestoreEnabled == true)
            {
                return;
            }

            foreach (KeyValuePair<IntPtr, WindowState> window in _Windows)
            {
                MoveWindow(window.Key, window.Value.Location);
            }

            _LayoutRestoreEnabled = true;
        }

        public void DisableLayoutRestore()
        {
            if (_LayoutRestoreEnabled == false)
            {
                return;
            }

            _LayoutRestoreEnabled = false;
        }

        internal void AddWindow(IntPtr hWnd)
        {
            if (_Windows.ContainsKey(hWnd) | IsWindowOwned(hWnd))
            {
                return;
            }

            WindowState windowState = InitializeWindow(hWnd);

            if (_LayoutRestoreEnabled)
            {
                MoveWindow(hWnd, windowState.Location);
            }

            if (_BlockingEnabled)
            {
                Block(hWnd, windowState);
            }

            _Windows.Add(hWnd, windowState);
        }

        private WindowState InitializeWindow(IntPtr hWnd)
        {
            string? className = GetWindowClassName(hWnd);

            if (className == null)
            {
                throw new Exception();
            }

            WindowState windowState;

            if (_WindowLayout.TryGetValue(className, out List<WindowLocation?>? locationPool))
            {
                for (int i = 0; i < locationPool.Count; i++)
                {
                    WindowLocation? locationInfo = locationPool![i];

                    if (locationInfo != null)
                    {
                        locationPool[i] = null;
                        windowState = new WindowState(locationPool, i, locationInfo);
                        return windowState;
                    }
                }

                locationPool.Add(null);
                windowState = new WindowState(locationPool, locationPool.Count, new WindowLocation(hWnd));
            }
            else
            {
                locationPool = new List<WindowLocation?> { null };
                _WindowLayout.Add(className, locationPool);
                windowState = new WindowState(locationPool, 0, new WindowLocation(hWnd));
            }

            return windowState;
        }

        private void RemoveWindow(IntPtr hWnd, WindowState windowState)
        {
            if (_BlockingEnabled)
            {
                Unblock(hWnd, windowState);
            }

            if (_LayoutRestoreEnabled)
            {
                SaveLayout(windowState);
            }
        }

        private void Block(IntPtr hWnd, WindowState windowState)
        {
            try
            {
                windowState.NoActivateSet = IsNoActivateSet(hWnd);
            }
            catch
            {

            }

            SetNoActivate(hWnd, true);
        }

        private void Unblock(IntPtr hWnd, WindowState windowState)
        {
            SetNoActivate(hWnd, windowState.NoActivateSet);
        }

        private void UpdateLocationInfo(IntPtr hWnd)
        {
            if (!_Windows.TryGetValue(hWnd, out WindowState? windowState))
            {
                return;
            }

            WindowLocation location = windowState.Location;
            location.IsMinimized = IsIconic(hWnd);

            if (location.IsMinimized)
            {
                return;
            }

            location.IsMaximized = IsZoomed(hWnd);

            try
            {
                location.WindowRect = GetWindowRect(hWnd);
            }
            catch { }
        }

        private void SaveLayout(WindowState windowState)
        {
            windowState.LocationPool[windowState.Slot] = windowState.Location;
        }

        public void Dispose()
        {
            _Process.Dispose();
            _WinEventListener?.Dispose();

            foreach (KeyValuePair<IntPtr, WindowState> window in _Windows)
            {
                RemoveWindow(window.Key, window.Value);
            }
        }

        private class WindowState
        {
            public List<WindowLocation?> LocationPool;
            public int Slot;
            public WindowLocation Location;
            public bool NoActivateSet;

            public WindowState(List<WindowLocation?> locationPool, int index, WindowLocation location)
            {
                LocationPool = locationPool;
                Slot = index;
                Location = location;
            }
        }
    }
}
