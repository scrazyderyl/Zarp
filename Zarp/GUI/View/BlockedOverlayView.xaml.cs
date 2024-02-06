using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using static Zarp.Common.Util.PInvoke;

namespace Zarp.GUI.View
{
    internal partial class BlockedOverlayView : Window
    {
        private IntPtr OverlayHandle;
        private IntPtr AttachedWindowHandle;
        private bool IsClosing = false;

        private IntPtr AttachedShowEvent;
        private IntPtr AttachedLocationChangeEvent;
        private IntPtr AttachedCloseEvent;

        private WinEventDelegate AttachedShowEventHandler;
        private WinEventDelegate AttachedLocationChangeEventHandler;
        private WinEventDelegate AttachedCloseEventHandler;

        public BlockedOverlayView(IntPtr attachedWindowHandle)
        {
            AttachedWindowHandle = attachedWindowHandle;
            AttachedShowEventHandler = OnAttachedShow;
            AttachedLocationChangeEventHandler = OnLocationChange;
            AttachedCloseEventHandler = OnAttachedClose;

            InitializeComponent();

            OverlayHandle = new WindowInteropHelper(this).EnsureHandle();
            long style = GetWindowLong(OverlayHandle, GWL_EXSTYLE);

            if (style == 0 || SetWindowLong(OverlayHandle, GWL_EXSTYLE, style | WS_EX_TOOLWINDOW) == 0)
            {
                throw new Exception();
            }

            if (GetWindowThreadProcessId(attachedWindowHandle, out uint processId) == 0 || processId == 0)
            {
                throw new Exception();
            }

            AttachedShowEvent = SetWinEventHook(EVENT_OBJECT_SHOW, EVENT_OBJECT_SHOW, IntPtr.Zero, AttachedShowEventHandler, processId, 0, WINEVENT_OUTOFCONTEXT);
            AttachedLocationChangeEvent = SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, AttachedLocationChangeEventHandler, processId, 0, WINEVENT_OUTOFCONTEXT);

            MoveOverlayToWindow();
            MoveOverlayOverWindow();
            Show();

            AttachedCloseEvent = SetWinEventHook(EVENT_OBJECT_DESTROY, EVENT_OBJECT_DESTROY, IntPtr.Zero, AttachedCloseEventHandler, processId, 0, WINEVENT_OUTOFCONTEXT);
        }

        protected override void OnActivated(EventArgs e)
        {
            MoveOverlayOverWindow();
            base.OnActivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (IsClosing)
            {
                return;
            }

            IsClosing = true;

            UnhookWinEvent(AttachedShowEvent);
            UnhookWinEvent(AttachedLocationChangeEvent);
            UnhookWinEvent(AttachedCloseEvent);

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Core.App.Service.Blocker.WindowClosed(AttachedWindowHandle);
            base.OnClosed(e);
        }

        internal void MoveOverlayOverWindow()
        {
            SetWindowPos(AttachedWindowHandle, OverlayHandle, 0, 0, 0, 0, SWP_ASYNCWINDOWPOS | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        internal void MoveOverlayToWindow()
        {
            System.Drawing.Rectangle? rect = GetWindowRect(AttachedWindowHandle);

            if (rect == null)
            {
                return;
            }

            Top = rect.Value.Top;
            Left = rect.Value.Left;
            Width = rect.Value.Width;
            Height = rect.Value.Height;
        }

        private void MinimizeAttachedWindow(object sender, RoutedEventArgs e)
        {
            MinimizeWindow(AttachedWindowHandle);
        }

        private void CloseAttachedWindow(object sender, RoutedEventArgs e)
        {
            CloseWindow(AttachedWindowHandle);
        }

        private void OnAttachedShow(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            if (hwnd == AttachedWindowHandle)
            {
                return;
            }

            MoveOverlayOverWindow();
        }

        private void OnLocationChange(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            if (hwnd != AttachedWindowHandle)
            {
                return;
            }

            MoveOverlayToWindow();
        }

        private void OnAttachedClose(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            if (hwnd != AttachedWindowHandle)
            {
                return;
            }

            Close();
        }
    }
}
