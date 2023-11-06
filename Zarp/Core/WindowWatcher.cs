using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Zarp.Core.PInvoke;

namespace Zarp.Core
{
    internal class WindowWatcher
    {
        IntPtr foregroundChangedEvent;

        public WindowWatcher()
        {
        
        }

        public void Enable()
        {
            foregroundChangedEvent = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, new WinEventDelegate(OnForegroundWindowChanged), 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public void Disable()
        {
            UnhookWinEvent(foregroundChangedEvent);
        }

        public void OnForegroundWindowChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {

        }
    }
}
