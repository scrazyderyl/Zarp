using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using static Zarp.Common.PInvoke.User32;
using static Zarp.Common.Util.Window;

namespace Zarp.GUI.View
{
    internal partial class BlockedOverlayView : Window
    {
        private IntPtr _OverlayHandle;
        private IntPtr _AttachedWindowHandle;
        private bool _IsClosing = false;

        private IntPtr _AttachedShowEvent;
        private IntPtr _AttachedLocationChangeEvent;
        private IntPtr _AttachedCloseEvent;

        private WinEventDelegate _AttachedShowEventHandler;
        private WinEventDelegate _AttachedLocationChangeEventHandler;
        private WinEventDelegate _AttachedCloseEventHandler;

        public BlockedOverlayView(IntPtr attachedWindowHandle)
        {
            _AttachedWindowHandle = attachedWindowHandle;
            _AttachedShowEventHandler = OnAttachedShow;
            _AttachedLocationChangeEventHandler = OnLocationChange;
            _AttachedCloseEventHandler = OnAttachedClose;

            InitializeComponent();

            _OverlayHandle = new WindowInteropHelper(this).EnsureHandle();
            long style = (long)GetWindowLongPtr(_OverlayHandle, GWL_EXSTYLE);

            if (style == 0 || SetWindowLongPtr(_OverlayHandle, GWL_EXSTYLE, new IntPtr(style | WS_EX_TOOLWINDOW)) == IntPtr.Zero)
            {
                throw new Exception();
            }

            if (GetWindowThreadProcessId(attachedWindowHandle, out uint processId) == 0 || processId == 0)
            {
                throw new Exception();
            }

            _AttachedShowEvent = SetWinEventHook(EVENT_OBJECT_SHOW, EVENT_OBJECT_SHOW, IntPtr.Zero, _AttachedShowEventHandler, processId, 0, WINEVENT_OUTOFCONTEXT);
            _AttachedLocationChangeEvent = SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, _AttachedLocationChangeEventHandler, processId, 0, WINEVENT_OUTOFCONTEXT);

            MoveOverlayToWindow();
            MoveOverlayOverWindow();
            Show();

            _AttachedCloseEvent = SetWinEventHook(EVENT_OBJECT_DESTROY, EVENT_OBJECT_DESTROY, IntPtr.Zero, _AttachedCloseEventHandler, processId, 0, WINEVENT_OUTOFCONTEXT);
        }

        protected override void OnActivated(EventArgs e)
        {
            MoveOverlayOverWindow();
            base.OnActivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_IsClosing)
            {
                return;
            }

            _IsClosing = true;

            UnhookWinEvent(_AttachedShowEvent);
            UnhookWinEvent(_AttachedLocationChangeEvent);
            UnhookWinEvent(_AttachedCloseEvent);

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        internal void MoveOverlayOverWindow()
        {
            SetWindowPos(_AttachedWindowHandle, _OverlayHandle, 0, 0, 0, 0, SWP_ASYNCWINDOWPOS | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        internal void MoveOverlayToWindow()
        {
            try
            {
                RECT rect = GetWindowRect(_AttachedWindowHandle);

                Top = rect.Top;
                Left = rect.Left;
                Width = rect.Right - rect.Left;
                Height = rect.Bottom - rect.Top;
            }
            catch { }
        }

        private void MinimizeAttachedWindow(object sender, RoutedEventArgs e)
        {
            MinimizeWindow(_AttachedWindowHandle);
        }

        private void CloseAttachedWindow(object sender, RoutedEventArgs e)
        {
            CloseWindow(_AttachedWindowHandle);
        }

        private void OnAttachedShow(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (hwnd == _AttachedWindowHandle)
            {
                return;
            }

            MoveOverlayOverWindow();
        }

        private void OnLocationChange(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (hwnd != _AttachedWindowHandle)
            {
                return;
            }

            MoveOverlayToWindow();
        }

        private void OnAttachedClose(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (hwnd != _AttachedWindowHandle)
            {
                return;
            }

            Close();
        }
    }
}
