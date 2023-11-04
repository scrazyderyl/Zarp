using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Zarp.Core;
using static Zarp.Core.PInvoke;

namespace Zarp
{
    /// <summary>
    /// Interaction logic for BlockedOverlay.xaml
    /// </summary>
    public partial class BlockedOverlay : Window
    {
        IntPtr AttachedWindowHandle;

        IntPtr MinimizeEvent;
        IntPtr UnminimizeEvent;
        IntPtr ActivateEvent;
        IntPtr MoveSizeStartEvent;
        IntPtr MoveSizeEndEvent;
        IntPtr CloseEvent;

        public BlockedOverlay(IntPtr attachedWindowHandle)
        {
            AttachedWindowHandle = attachedWindowHandle;

            InitializeComponent();
            Show();

            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            int style = WS_EX_TOOLWINDOW | GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, style);
            GetWindowThreadProcessId(hWnd, out uint processId);

            MoveOverlayToWindow();

            MinimizeEvent = SetWinEventHook(EVENT_SYSTEM_MINIMIZESTART, EVENT_SYSTEM_MINIMIZESTART, IntPtr.Zero, new WinEventDelegate(OnAttachedMinimize), processId, 0, WINEVENT_OUTOFCONTEXT);
            UnminimizeEvent = SetWinEventHook(EVENT_SYSTEM_MINIMIZEEND, EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, new WinEventDelegate(OnAttachedUnminimize), processId, 0, WINEVENT_OUTOFCONTEXT);
            ActivateEvent = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, new WinEventDelegate(OnAttachedActivate), processId, 0, WINEVENT_OUTOFCONTEXT);
            MoveSizeStartEvent = SetWinEventHook(EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZESTART, IntPtr.Zero, new WinEventDelegate(OnAttachedMoveSizeStart), processId, 0, WINEVENT_OUTOFCONTEXT);
            MoveSizeStartEvent = SetWinEventHook(EVENT_SYSTEM_MOVESIZEEND, EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, new WinEventDelegate(OnAttachedMoveSizeEnd), processId, 0, WINEVENT_OUTOFCONTEXT);
            CloseEvent = SetWinEventHook(EVENT_OBJECT_DESTROY, EVENT_OBJECT_DESTROY, IntPtr.Zero, new WinEventDelegate(OnAttachedClose), processId, 0, WINEVENT_OUTOFCONTEXT);
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
            MoveOverlayToWindow();
            Show();
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
            Close();
        }
    }
}
