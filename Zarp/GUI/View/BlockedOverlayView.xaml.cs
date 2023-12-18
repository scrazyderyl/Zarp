using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using static Zarp.Common.Util.PInvoke;

namespace Zarp.GUI.View
{
    /// <summary>
    /// Interaction logic for BlockedOverlay.xaml
    /// </summary>
    public partial class BlockedOverlayView : Window
    {
        IntPtr AttachedWindowHandle;

        IntPtr MinimizeEvent;
        IntPtr UnminimizeEvent;
        IntPtr ActivateEvent;
        IntPtr MoveSizeStartEvent;
        IntPtr MoveSizeEndEvent;
        IntPtr CloseEvent;

        public BlockedOverlayView(IntPtr attachedWindowHandle)
        {
            AttachedWindowHandle = attachedWindowHandle;

            InitializeComponent();
            Show();

            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            int style = WS_EX_TOOLWINDOW | GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, style);
            GetWindowThreadProcessId(hWnd, out uint processId);

            MoveOverlayToWindow();

            MinimizeEvent = SubscribeWinEvent(EVENT_SYSTEM_MINIMIZESTART, new WinEventDelegate(OnAttachedMinimize), processId);
            UnminimizeEvent = SubscribeWinEvent(EVENT_SYSTEM_MINIMIZEEND, new WinEventDelegate(OnAttachedUnminimize), processId);
            ActivateEvent = SubscribeWinEvent(EVENT_SYSTEM_FOREGROUND, new WinEventDelegate(OnAttachedActivate), processId);
            MoveSizeStartEvent = SubscribeWinEvent(EVENT_SYSTEM_MOVESIZESTART, new WinEventDelegate(OnAttachedMoveSizeStart), processId);
            MoveSizeEndEvent = SubscribeWinEvent(EVENT_SYSTEM_MOVESIZEEND, new WinEventDelegate(OnAttachedMoveSizeEnd), processId);
            CloseEvent = SubscribeWinEvent(EVENT_OBJECT_DESTROY, new WinEventDelegate(OnAttachedClose), processId);

            Activate();
        }

        void MinimizeAttachedWindow(object sender, RoutedEventArgs e)
        {
            MinimizeWindow(AttachedWindowHandle);
        }

        void CloseAttachedWindow(object sender, RoutedEventArgs e)
        {
            DestroyWindow(AttachedWindowHandle);
        }

        void MoveOverlayToWindow()
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

        void OnClose(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        void OnAttachedMinimize(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            Hide();
        }

        void OnAttachedUnminimize(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            Show();
        }

        void OnAttachedActivate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            MoveOverlayToWindow();
            Activate();
        }

        void OnAttachedMoveSizeStart(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            Hide();
        }

        void OnAttachedMoveSizeEnd(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            Show();
            MoveOverlayToWindow();
            Activate();
        }

        void OnAttachedClose(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            UnhookWinEvent(MinimizeEvent);
            UnhookWinEvent(UnminimizeEvent);
            UnhookWinEvent(ActivateEvent);
            UnhookWinEvent(MoveSizeStartEvent);
            UnhookWinEvent(MoveSizeEndEvent);
            UnhookWinEvent(CloseEvent);
            Core.Service.Zarp.Blocker.WindowClosed(AttachedWindowHandle);
            Close();
        }
    }
}
